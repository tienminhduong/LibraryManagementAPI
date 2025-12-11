using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class BorrowRequestService(
        IBorrowRequestRepository borrowRequestRepo,
        IBookCopyRepository bookCopyRepo,
        IBookRepository bookRepo,
        IBookTransactionRepository transactionRepo,
        IInfoRepository infoRepo,
        ICartRepository cartRepo,
        IUnitOfWork uow) : IBorrowRequestService
    {
        public async Task<BorrowRequestResponseDto> CreateBorrowRequestAsync(CreateBorrowRequestDto dto, Guid accountId)
        {
            // 1. Get member info from account ID
            var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                throw new Exception("Member information not found.");

            var memberId = memberInfo.id;

            // 2. Validate all books exist and have available copies
            foreach (var bookId in dto.BookIds)
            {
                // Check if book exists
                var bookExists = await bookRepo.IsBookExistsByIdAsync(bookId);
                if (!bookExists)
                    throw new Exception($"Book with ID {bookId} not found.");

                // Check if book has available copies
                var hasAvailableCopies = await bookCopyRepo.HasAvailableCopiesForBook(bookId);
                if (!hasAvailableCopies)
                    throw new Exception($"Book with ID {bookId} has no available copies.");
            }

            // 3. Generate QR code (using request ID)
            var requestId = Guid.NewGuid();
            var qrCode = GenerateQrCode(requestId);

            // 4. Create borrow request
            var borrowRequest = new BorrowRequest
            {
                Id = requestId,
                MemberId = memberId,
                QrCode = qrCode,
                Notes = dto.Notes,
                Status = BorrowRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // 5. Add request items (with BookIds, not BookCopyIds yet)
            foreach (var bookId in dto.BookIds)
            {
                borrowRequest.Items.Add(new BorrowRequestItem
                {
                    Id = Guid.NewGuid(),
                    BorrowRequestId = requestId,
                    BookId = bookId
                });
            }

            // 6. Save to database and remove books from cart
            await uow.BeginTransactionAsync();
            try
            {
                await borrowRequestRepo.Add(borrowRequest);
                
                // Remove the requested books from the cart
                var cart = await cartRepo.GetByAccountIdAsync(accountId);
                if (cart != null)
                {
                    // Find cart items that match the requested books
                    var cartItemsToRemove = cart.Items
                        .Where(ci => dto.BookIds.Contains(ci.BookId))
                        .ToList();

                    // Remove each matching cart item
                    foreach (var cartItem in cartItemsToRemove)
                    {
                        await cartRepo.RemoveItemAsync(cartItem.Id);
                    }

                    // Update cart timestamp if items were removed
                    if (cartItemsToRemove.Any())
                    {
                        cart.UpdatedAt = DateTime.UtcNow;
                        await cartRepo.UpdateAsync(cart);
                    }
                }
                
                await uow.SaveChangesAsync();
                await uow.CommitAsync();

                return new BorrowRequestResponseDto
                {
                    RequestId = requestId,
                    QrCode = qrCode,
                    Message = "Borrow request created successfully. Please show this QR code to the librarian."
                };
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while creating the borrow request.", ex);
            }
        }

        public async Task<BorrowRequestDto?> GetBorrowRequestByIdAsync(Guid id, Guid accountId, string userRole)
        {
            var request = await borrowRequestRepo.GetByIdWithDetails(id);
            if (request == null)
                return null;

            // Members can only view their own requests
            if (userRole.Equals("Member", StringComparison.OrdinalIgnoreCase))
            {
                var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
                if (memberInfo == null || request.MemberId != memberInfo.id)
                    return null; // Access denied
            }

            return MapToDto(request);
        }

        public async Task<BorrowRequestDto?> GetBorrowRequestByQrCodeAsync(string qrCode)
        {
            var requests = await borrowRequestRepo.GetAll();
            var request = requests.FirstOrDefault(r => r.QrCode == qrCode);
            
            if (request == null)
                return null;

            return MapToDto(request);
        }

        public async Task<bool> ConfirmBorrowRequestAsync(ConfirmBorrowRequestDto dto, Guid staffAccountId)
        {
            // 1. Get staff info from account ID
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            var staffId = staffInfo.id;

            // 2. Get borrow request
            var request = await borrowRequestRepo.GetByIdWithDetails(dto.RequestId);
            if (request == null)
                throw new Exception("Borrow request not found.");

            if (request.Status != BorrowRequestStatus.Pending)
                throw new Exception("Borrow request has already been processed.");

            await uow.BeginTransactionAsync();
            try
            {
                // 3. Assign book copies and create transactions
                foreach (var assignment in dto.BookCopyAssignments)
                {
                    var item = request.Items.FirstOrDefault(i => i.BookId == assignment.BookId);
                    if (item == null)
                        continue;

                    // Get and validate book copy
                    var bookCopy = await bookCopyRepo.GetById(assignment.BookCopyId);
                    if (bookCopy == null || bookCopy.status != Status.Available)
                        throw new Exception($"Book copy {assignment.BookCopyId} is not available.");

                    // Update book copy status
                    bookCopy.status = Status.Borrowed;
                    await bookCopyRepo.Update(bookCopy);

                    // Update request item
                    item.BookCopyId = assignment.BookCopyId;
                    item.IsConfirmed = true;

                    // Create book transaction
                    var transaction = new BookTransaction
                    {
                        id = Guid.NewGuid(),
                        copyId = assignment.BookCopyId,
                        memberId = request.MemberId,
                        staffId = staffId,
                        borrowDate = DateTime.UtcNow,
                        dueDate = DateTime.UtcNow.AddDays(30),
                        status = StatusTransaction.BORROWED
                    };

                    await transactionRepo.Add(transaction);
                }

                // 4. Update borrow request status
                request.Status = BorrowRequestStatus.Confirmed;
                request.StaffId = staffId;
                request.ConfirmedAt = DateTime.UtcNow;
                request.DueDate = DateTime.UtcNow.AddDays(30); // Set due date to 30 days from confirmation
                await borrowRequestRepo.Update(request);

                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while confirming the borrow request.", ex);
            }
        }

        public async Task<bool> RejectBorrowRequestAsync(Guid requestId, Guid staffAccountId, string reason)
        {
            // 1. Get staff info from account ID
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            var staffId = staffInfo.id;

            // 2. Get borrow request
            var request = await borrowRequestRepo.GetById(requestId);
            if (request == null)
                throw new Exception("Borrow request not found.");

            if (request.Status != BorrowRequestStatus.Pending)
                throw new Exception("Borrow request has already been processed.");

            // 3. Update request status
            request.Status = BorrowRequestStatus.Rejected;
            request.StaffId = staffId;
            request.Notes = $"{request.Notes}\nRejection reason: {reason}";

            await uow.BeginTransactionAsync();
            try
            {
                await borrowRequestRepo.Update(request);
                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while rejecting the borrow request.", ex);
            }
        }

        public async Task<bool> CancelBorrowRequestAsync(Guid requestId, Guid memberAccountId)
        {
            // 1. Get member info from account ID
            var memberInfo = await infoRepo.GetByAccountIdAsync(memberAccountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                throw new Exception("Member information not found.");

            var request = await borrowRequestRepo.GetById(requestId);
            if (request == null)
                throw new Exception("Borrow request not found.");

            if (request.MemberId != memberInfo.id)
                throw new Exception("You are not authorized to cancel this request.");

            if (request.Status != BorrowRequestStatus.Pending)
                throw new Exception("Only pending requests can be cancelled.");

            request.Status = BorrowRequestStatus.Cancelled;

            await uow.BeginTransactionAsync();
            try
            {
                await borrowRequestRepo.Update(request);
                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while cancelling the borrow request.", ex);
            }
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetPendingRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Pending, pageNumber, pageSize);
            var dtoList = paged.Data.Select(MapToDto);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsPagedAsync(Guid memberAccountId, int pageNumber = 1, int pageSize = 20)
        {
            // Get member info from account ID
            var memberInfo = await infoRepo.GetByAccountIdAsync(memberAccountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                return new PagedResponse<BorrowRequestDto>(pageNumber, pageSize, Enumerable.Empty<BorrowRequestDto>(), 0);

            var paged = await borrowRequestRepo.GetByMemberIdPaged(memberInfo.id, pageNumber, pageSize);
            var dtoList = paged.Data.Select(MapToDto);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsByInfoIdPagedAsync(Guid memberInfoId, int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByMemberIdPaged(memberInfoId, pageNumber, pageSize);
            var dtoList = paged.Data.Select(MapToDto);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<ReturnBookResultDto> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId)
        {
            // 1. Get staff info from account ID
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            // 2. Get book copy
            var bookCopy = await bookCopyRepo.GetById(dto.BookCopyId);
            if (bookCopy == null)
                throw new Exception("Book copy not found.");

            if (bookCopy.status != Status.Borrowed)
                throw new Exception("This book is not currently borrowed.");

            await uow.BeginTransactionAsync();
            try
            {
                // 3. Find active transaction for this book copy
                var transactions = await transactionRepo.GetAll();
                var activeTransaction = transactions.FirstOrDefault(t => 
                    t.copyId == dto.BookCopyId && 
                    t.status == StatusTransaction.BORROWED);

                if (activeTransaction == null)
                    throw new Exception("No active transaction found for this book copy.");

                // 4. Update transaction
                activeTransaction.returnDate = DateTime.UtcNow;
                activeTransaction.status = StatusTransaction.RETURNED;
                await transactionRepo.Update(activeTransaction);

                // 5. Update book copy status
                bookCopy.status = Status.Available;
                await bookCopyRepo.Update(bookCopy);

                await uow.SaveChangesAsync();
                await uow.CommitAsync();

                // 6. Get student name and book title
                string? studentName = null;
                string? bookTitle = null;

                if (activeTransaction.memberId != Guid.Empty)
                {
                    var memberInfo = await infoRepo.GetByIdAsync(activeTransaction.memberId) as MemberInfo;
                    studentName = memberInfo?.fullName;
                }

                bookTitle = bookCopy.book?.Title;

                return new ReturnBookResultDto
                {
                    Success = true,
                    Message = "Book returned successfully.",
                    StudentName = studentName,
                    BookTitle = bookTitle
                };
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while returning the book.", ex);
            }
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetBorrowedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            // Get all confirmed requests that are currently borrowed (not overdue)
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Confirmed, pageNumber, pageSize);
            var currentTime = DateTime.UtcNow;
            
            // Filter to only borrowed (not overdue) requests
            var borrowedRequests = paged.Data
                .Where(r => r.DueDate.HasValue && r.DueDate.Value >= currentTime)
                .ToList();
            
            var dtoList = borrowedRequests.Select(MapToDto);
            // Recalculate total based on filtered results
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, borrowedRequests.Count);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetOverdueRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            // Get all confirmed requests that are overdue
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Confirmed, pageNumber, pageSize);
            var currentTime = DateTime.UtcNow;
            
            // Filter to only overdue requests
            var overdueRequests = paged.Data
                .Where(r => r.DueDate.HasValue && r.DueDate.Value < currentTime)
                .ToList();
            
            var dtoList = overdueRequests.Select(MapToDto);
            // Recalculate total based on filtered results
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, overdueRequests.Count);
        }

        public async Task<BorrowRequestResponseDto> AdminCreateBorrowRequestAsync(AdminCreateBorrowRequestDto dto, Guid staffAccountId)
        {
            // 1. Get staff info to verify authorization
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            var staffId = staffInfo.id;

            // 2. Validate member exists
            var memberInfo = await infoRepo.GetByIdAsync(dto.MemberId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                throw new Exception("Member not found.");

            // 3. Validate all book copies exist and are available
            var bookCopies = new List<BookCopy>();
            foreach (var bookCopyId in dto.BookCopyIds)
            {
                var bookCopy = await bookCopyRepo.GetById(bookCopyId);
                if (bookCopy == null)
                    throw new Exception($"Book copy with ID {bookCopyId} not found.");
                
                if (bookCopy.status != Status.Available)
                    throw new Exception($"Book copy with ID {bookCopyId} is not available.");
                
                bookCopies.Add(bookCopy);
            }

            // 4. Generate QR code
            var requestId = Guid.NewGuid();
            var qrCode = GenerateQrCode(requestId);

            // 5. Create borrow request with Confirmed status and assign book copies immediately
            var borrowRequest = new BorrowRequest
            {
                Id = requestId,
                MemberId = dto.MemberId,
                StaffId = staffId,
                QrCode = qrCode,
                Notes = dto.Notes,
                Status = BorrowRequestStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            await uow.BeginTransactionAsync();
            try
            {
                // 6. Add request items and update book copies
                foreach (var bookCopy in bookCopies)
                {
                    // Add request item
                    borrowRequest.Items.Add(new BorrowRequestItem
                    {
                        Id = Guid.NewGuid(),
                        BorrowRequestId = requestId,
                        BookId = bookCopy.bookId,
                        BookCopyId = bookCopy.id,
                        IsConfirmed = true
                    });

                    // Update book copy status to Borrowed
                    bookCopy.status = Status.Borrowed;
                    await bookCopyRepo.Update(bookCopy);

                    // Create book transaction
                    var transaction = new BookTransaction
                    {
                        id = Guid.NewGuid(),
                        copyId = bookCopy.id,
                        memberId = dto.MemberId,
                        staffId = staffId,
                        borrowDate = DateTime.UtcNow,
                        dueDate = DateTime.UtcNow.AddDays(30),
                        status = StatusTransaction.BORROWED
                    };

                    await transactionRepo.Add(transaction);
                }

                // 7. Save borrow request
                await borrowRequestRepo.Add(borrowRequest);

                await uow.SaveChangesAsync();
                await uow.CommitAsync();

                return new BorrowRequestResponseDto
                {
                    RequestId = requestId,
                    QrCode = qrCode,
                    Message = "Borrow request created and confirmed successfully by admin."
                };
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while creating the admin borrow request.", ex);
            }
        }

        public async Task<IEnumerable<MemberSearchDto>> SearchMembersAsync(string searchTerm)
        {
            var members = await infoRepo.SearchMembersAsync(searchTerm);
            
            return members.Select(m => new MemberSearchDto
            {
                Id = m.id,
                FullName = m.fullName,
                Email = m.email,
                PhoneNumber = m.phoneNumber,
                Address = m.address
            });
        }

        public async Task<PagedResponse<BorrowHistoryDto>> GetReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            // Get all confirmed requests
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Confirmed, pageNumber, pageSize);
            
            // Get all transactions to check return status
            var allTransactions = await transactionRepo.GetAll();
            
            // Filter to only fully returned requests
            var returnedRequests = new List<BorrowRequest>();
            foreach (var request in paged.Data)
            {
                // Check if all book copies in this request have been returned
                var allReturned = request.Items.All(item =>
                {
                    if (!item.BookCopyId.HasValue) return false;
                    
                    var transaction = allTransactions.FirstOrDefault(t =>
                        t.copyId == item.BookCopyId.Value &&
                        t.memberId == request.MemberId);
                    
                    return transaction != null && transaction.status == StatusTransaction.RETURNED;
                });
                
                if (allReturned && request.Items.Any())
                {
                    returnedRequests.Add(request);
                }
            }
            
            var dtoList = returnedRequests.Select(r => MapToHistoryDto(r, allTransactions));
            return new PagedResponse<BorrowHistoryDto>(paged.PageNumber, paged.PageSize, dtoList, returnedRequests.Count);
        }

        public async Task<PagedResponse<BorrowHistoryDto>> GetMemberReturnedRequestsPagedAsync(Guid memberAccountId, int pageNumber = 1, int pageSize = 20)
        {
            // Get member info from account ID
            var memberInfo = await infoRepo.GetByAccountIdAsync(memberAccountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                return new PagedResponse<BorrowHistoryDto>(pageNumber, pageSize, Enumerable.Empty<BorrowHistoryDto>(), 0);

            // Get member's confirmed requests
            var paged = await borrowRequestRepo.GetByMemberIdPaged(memberInfo.id, pageNumber, pageSize);
            var confirmedRequests = paged.Data.Where(r => r.Status == BorrowRequestStatus.Confirmed).ToList();
            
            // Get all transactions
            var allTransactions = await transactionRepo.GetAll();
            
            // Filter to only fully returned requests
            var returnedRequests = new List<BorrowRequest>();
            foreach (var request in confirmedRequests)
            {
                // Check if all book copies in this request have been returned
                var allReturned = request.Items.All(item =>
                {
                    if (!item.BookCopyId.HasValue) return false;
                    
                    var transaction = allTransactions.FirstOrDefault(t =>
                        t.copyId == item.BookCopyId.Value &&
                        t.memberId == request.MemberId);
                    
                    return transaction != null && transaction.status == StatusTransaction.RETURNED;
                });
                
                if (allReturned && request.Items.Any())
                {
                    returnedRequests.Add(request);
                }
            }
            
            var dtoList = returnedRequests.Select(r => MapToHistoryDto(r, allTransactions));
            return new PagedResponse<BorrowHistoryDto>(paged.PageNumber, paged.PageSize, dtoList, returnedRequests.Count);
        }

        private string GenerateQrCode(Guid requestId)
        {
            // Generate a QR code data string
            return $"BORROW-{requestId}";
        }

        private BorrowRequestDto MapToDto(BorrowRequest request)
        {
            return new BorrowRequestDto
            {
                Id = request.Id,
                MemberId = request.MemberId,
                MemberName = request.Member?.fullName,
                MemberEmail = request.Member?.email,
                StaffId = request.StaffId,
                StaffName = request.Staff?.fullName,
                CreatedAt = request.CreatedAt,
                ConfirmedAt = request.ConfirmedAt,
                DueDate = request.DueDate,
                Status = request.Status.ToString(),
                QrCode = request.QrCode,
                Notes = request.Notes,
                Items = request.Items.Select(i => new BorrowRequestItemDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = i.Book?.Title,
                    BookISBN = i.Book?.ISBN,
                    BookCopyId = i.BookCopyId,
                    IsConfirmed = i.IsConfirmed
                }).ToList()
            };
        }

        private BorrowHistoryDto MapToHistoryDto(BorrowRequest request, IEnumerable<BookTransaction> transactions)
        {
            var requestTransactions = request.Items
                .Where(i => i.BookCopyId.HasValue)
                .Select(i => transactions.FirstOrDefault(t => 
                    t.copyId == i.BookCopyId.Value && 
                    t.memberId == request.MemberId))
                .Where(t => t != null)
                .ToList();
            
            var latestReturnDate = requestTransactions
                .Where(t => t.returnDate.HasValue)
                .Max(t => t.returnDate);
            
            var wasOverdue = requestTransactions.Any(t => 
                t.returnDate.HasValue && 
                request.DueDate.HasValue && 
                t.returnDate.Value > request.DueDate.Value);
            
            return new BorrowHistoryDto
            {
                Id = request.Id,
                MemberId = request.MemberId,
                MemberName = request.Member?.fullName,
                MemberEmail = request.Member?.email,
                StaffId = request.StaffId,
                StaffName = request.Staff?.fullName,
                CreatedAt = request.CreatedAt,
                ConfirmedAt = request.ConfirmedAt,
                DueDate = request.DueDate,
                ReturnedAt = latestReturnDate,
                Status = request.Status.ToString(),
                QrCode = request.QrCode,
                Notes = request.Notes,
                IsFullyReturned = true,
                WasOverdue = wasOverdue,
                Items = request.Items.Select(i =>
                {
                    var transaction = i.BookCopyId.HasValue 
                        ? transactions.FirstOrDefault(t => t.copyId == i.BookCopyId.Value && t.memberId == request.MemberId)
                        : null;
                    
                    return new BorrowHistoryItemDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = i.Book?.Title,
                        BookISBN = i.Book?.ISBN,
                        BookCopyId = i.BookCopyId,
                        BorrowDate = transaction?.borrowDate,
                        ReturnDate = transaction?.returnDate,
                        IsReturned = transaction?.status == StatusTransaction.RETURNED
                    };
                }).ToList()
            };
        }
    }
}
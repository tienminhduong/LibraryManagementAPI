using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.BorrowRequest;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class BorrowRequestService(
        IBorrowRequestRepository borrowRequestRepo,
        IBookCopyRepository bookCopyRepo,
        IBookRepository bookRepo,
        IBookTransactionRepository transactionRepo,
        IInfoRepository infoRepo,
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

            // 6. Save to database
            await uow.BeginTransactionAsync();
            try
            {
                await borrowRequestRepo.Add(borrowRequest);
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

        public async Task<IEnumerable<BorrowRequestDto>> GetPendingRequestsAsync()
        {
            var requests = await borrowRequestRepo.GetByStatus(BorrowRequestStatus.Pending);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<BorrowRequestDto>> GetMemberRequestsAsync(Guid memberAccountId)
        {
            // Get member info from account ID
            var memberInfo = await infoRepo.GetByAccountIdAsync(memberAccountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                return Enumerable.Empty<BorrowRequestDto>();

            var requests = await borrowRequestRepo.GetByMemberId(memberInfo.id);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<BorrowRequestDto>> GetMemberRequestsByInfoIdAsync(Guid memberInfoId)
        {
            var requests = await borrowRequestRepo.GetByMemberId(memberInfoId);
            return requests.Select(MapToDto);
        }

        public async Task<bool> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId)
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
                return true;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while returning the book.", ex);
            }
        }

        private string GenerateQrCode(Guid requestId)
        {
            // Generate a QR code data string
            // In a real implementation, you would use a QR code library
            // For now, we'll just encode the request ID
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
    }
}

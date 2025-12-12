using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Services
{
    public class BorrowRequestService(
        IBorrowRequestRepository borrowRequestRepo,
        IBookCopyRepository bookCopyRepo,
        IBookRepository bookRepo,
        IInfoRepository infoRepo,
        ICartRepository cartRepo,
        IUnitOfWork uow,
        IAccountRepository accountRepo) : IBorrowRequestService
    {
        /// <summary>
        /// Creates MULTIPLE borrow requests (one per book)
        /// </summary>
        public async Task<BorrowRequestResponseDto> CreateBorrowRequestAsync(CreateBorrowRequestDto dto, Guid accountId)
        {
            var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                throw new Exception("Member information not found.");

            var memberId = memberInfo.id;
            var requestIds = new List<Guid>();
            var qrCodes = new List<string>();

            await uow.BeginTransactionAsync();
            try
            {
                // Create ONE request per book
                foreach (var bookId in dto.BookIds)
                {
                    // Validate book exists
                    var bookExists = await bookRepo.IsBookExistsByIdAsync(bookId);
                    if (!bookExists)
                        throw new Exception($"Book with ID {bookId} not found.");

                    // Check if book has available copies
                    var hasAvailableCopies = await bookCopyRepo.HasAvailableCopiesForBook(bookId);
                    if (!hasAvailableCopies)
                        throw new Exception($"Book with ID {bookId} has no available copies.");

                    // Create separate request for this book
                    var requestId = Guid.NewGuid();
                    var qrCode = GenerateQrCode(requestId);

                    var borrowRequest = new BorrowRequest
                    {
                        Id = requestId,
                        MemberId = memberId,
                        BookId = bookId,
                        BookCopyId = null, // Assigned later by staff
                        QrCode = qrCode,
                        Notes = dto.Notes,
                        Status = BorrowRequestStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    await borrowRequestRepo.Add(borrowRequest);
                    requestIds.Add(requestId);
                    qrCodes.Add(qrCode);
                }

                // Remove books from cart
                var cart = await cartRepo.GetByAccountIdAsync(accountId);
                if (cart != null)
                {
                    var cartItemsToRemove = cart.Items
                        .Where(ci => dto.BookIds.Contains(ci.BookId))
                        .ToList();

                    foreach (var cartItem in cartItemsToRemove)
                    {
                        await cartRepo.RemoveItemAsync(cartItem.Id);
                    }

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
                    RequestIds = requestIds,
                    QrCodes = qrCodes,
                    Message = $"{requestIds.Count} borrow request(s) created successfully. Show QR codes to librarian.",
                    TotalRequests = requestIds.Count
                };
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while creating borrow requests.", ex);
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
                    return null;
            }

            return await MapToDtoAsync(request);
        }

        public async Task<BorrowRequestDto?> GetBorrowRequestByQrCodeAsync(string qrCode)
        {
            var request = await borrowRequestRepo.GetByQrCodeAsync(qrCode);
            
            if (request == null)
                return null;

            return await MapToDtoAsync(request);
        }

        /// <summary>
        /// Confirms a SINGLE borrow request with a specific book copy
        /// </summary>
        public async Task<bool> ConfirmBorrowRequestAsync(ConfirmBorrowRequestDto dto, Guid staffAccountId)
        {
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            // Only assign StaffId if the actor is a StaffInfo (not Admin)
            Guid? staffInfoId = staffInfo is StaffInfo s ? s.id : (Guid?)null;

            var request = await borrowRequestRepo.GetByIdWithDetails(dto.RequestId);
            if (request == null)
                throw new Exception("Borrow request not found.");

            if (request.Status != BorrowRequestStatus.Pending)
                throw new Exception("Borrow request has already been processed.");

            await uow.BeginTransactionAsync();
            try
            {
                // Get and validate book copy
                var bookCopy = await bookCopyRepo.GetById(dto.BookCopyId);
                if (bookCopy == null || bookCopy.status != Status.Available)
                    throw new Exception($"Book copy {dto.BookCopyId} is not available.");

                // Verify book copy matches requested book
                if (bookCopy.bookId != request.BookId)
                    throw new Exception($"Book copy does not match the requested book.");

                // Update book copy status
                bookCopy.status = Status.Borrowed;
                await bookCopyRepo.Update(bookCopy);

                // Update request
                request.BookCopyId = dto.BookCopyId;
                request.Status = BorrowRequestStatus.Borrowed;
                request.StaffId = staffInfoId;
                request.ProcessedByAccountId = staffAccountId; // record acting account (admin or staff)
                request.ConfirmedAt = DateTime.UtcNow;
                request.BorrowDate = DateTime.UtcNow;
                request.DueDate = DateTime.UtcNow.AddDays(30);
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
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            Guid? staffInfoId = staffInfo is StaffInfo s ? s.id : (Guid?)null;

            var request = await borrowRequestRepo.GetById(requestId);
            if (request == null)
                throw new Exception("Borrow request not found.");

            if (request.Status != BorrowRequestStatus.Pending)
                throw new Exception("Borrow request has already been processed.");

            request.Status = BorrowRequestStatus.Rejected;
            request.StaffId = staffInfoId;
            request.ProcessedByAccountId = staffAccountId; // record acting account
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

        /// <summary>
        /// Returns a book copy and updates the corresponding borrow request
        /// </summary>
        public async Task<ReturnBookResultDto> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId)
        {
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            var bookCopy = await bookCopyRepo.GetById(dto.BookCopyId);
            if (bookCopy == null)
                throw new Exception("Book copy not found.");

            if (bookCopy.status != Status.Borrowed)
                throw new Exception("This book is not currently borrowed.");

            // Find the active borrow request for this book copy
            var borrowRequest = await borrowRequestRepo.GetByBookCopyIdAsync(dto.BookCopyId);
            if (borrowRequest == null)
                throw new Exception("No active borrow request found for this book copy.");

            await uow.BeginTransactionAsync();
            try
            {
                // Update book copy status
                bookCopy.status = Status.Available;
                await bookCopyRepo.Update(bookCopy);

                // Update borrow request
                borrowRequest.ReturnedAt = DateTime.UtcNow;
                borrowRequest.ProcessedByAccountId = staffAccountId; // record acting account
                var wasOverdue = borrowRequest.DueDate.HasValue && 
                                borrowRequest.ReturnedAt.Value > borrowRequest.DueDate.Value;
                
                borrowRequest.Status = wasOverdue 
                    ? BorrowRequestStatus.OverdueReturned 
                    : BorrowRequestStatus.Returned;
                
                await borrowRequestRepo.Update(borrowRequest);

                await uow.SaveChangesAsync();
                await uow.CommitAsync();

                return new ReturnBookResultDto
                {
                    Success = true,
                    Message = "Book returned successfully.",
                    MemberName = borrowRequest.Member?.fullName,
                    BookTitle = borrowRequest.Book?.Title,
                    BorrowRequestId = borrowRequest.Id,
                    ReturnedAt = borrowRequest.ReturnedAt,
                    WasOverdue = wasOverdue
                };
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new Exception("An error occurred while returning the book.", ex);
            }
        }

        /// <summary>
        /// Admin creates and immediately confirms a borrow request
        /// </summary>
        public async Task<BorrowRequestResponseDto> AdminCreateBorrowRequestAsync(AdminCreateBorrowRequestDto dto, Guid staffAccountId)
        {
            var staffInfo = await infoRepo.GetByAccountIdAsync(staffAccountId);
            if (staffInfo == null || (staffInfo is not StaffInfo && staffInfo is not AdminInfo))
                throw new Exception("Staff information not found.");

            Guid? staffInfoId = staffInfo is StaffInfo s ? s.id : (Guid?)null;

            var staffId = staffInfoId; // for readability

            // Validate member exists
            var memberInfo = await infoRepo.GetByIdAsync(dto.MemberId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                throw new Exception("Member not found.");

            // Validate book copy exists and is available
            var bookCopy = await bookCopyRepo.GetById(dto.BookCopyId);
            if (bookCopy == null)
                throw new Exception($"Book copy with ID {dto.BookCopyId} not found.");
            
            if (bookCopy.status != Status.Available)
                throw new Exception($"Book copy with ID {dto.BookCopyId} is not available.");

            var requestId = Guid.NewGuid();
            var qrCode = GenerateQrCode(requestId);

            // Create request with Borrowed status (immediately confirmed)
            var borrowRequest = new BorrowRequest
            {
                Id = requestId,
                MemberId = dto.MemberId,
                StaffId = staffId,
                ProcessedByAccountId = staffAccountId, // record acting account
                BookId = bookCopy.bookId,
                BookCopyId = dto.BookCopyId,
                QrCode = qrCode,
                Notes = dto.Notes,
                Status = BorrowRequestStatus.Borrowed,
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = DateTime.UtcNow,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            await uow.BeginTransactionAsync();
            try
            {
                // Update book copy status
                bookCopy.status = Status.Borrowed;
                await bookCopyRepo.Update(bookCopy);

                // Save borrow request
                await borrowRequestRepo.Add(borrowRequest);

                await uow.SaveChangesAsync();
                await uow.CommitAsync();

                return new BorrowRequestResponseDto
                {
                    RequestIds = new List<Guid> { requestId },
                    QrCodes = new List<string> { qrCode },
                    Message = "Borrow request created and confirmed successfully by admin.",
                    TotalRequests = 1
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

        public async Task<PagedResponse<BorrowRequestDto>> GetPendingRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Pending, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetBorrowedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Borrowed, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetOverdueRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Overdue, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.Returned, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetOverdueReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByStatusPaged(BorrowRequestStatus.OverdueReturned, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsPagedAsync(Guid memberAccountId, int pageNumber = 1, int pageSize = 20)
        {
            var memberInfo = await infoRepo.GetByAccountIdAsync(memberAccountId);
            if (memberInfo == null || memberInfo is not MemberInfo)
                return new PagedResponse<BorrowRequestDto>(pageNumber, pageSize, Enumerable.Empty<BorrowRequestDto>(), 0);

            var paged = await borrowRequestRepo.GetByMemberIdPaged(memberInfo.id, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        public async Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsByInfoIdPagedAsync(Guid memberInfoId, int pageNumber = 1, int pageSize = 20)
        {
            var paged = await borrowRequestRepo.GetByMemberIdPaged(memberInfoId, pageNumber, pageSize);
            var dtoTasks = paged.Data.Select(r => MapToDtoAsync(r));
            var dtoList = await Task.WhenAll(dtoTasks);
            return new PagedResponse<BorrowRequestDto>(paged.PageNumber, paged.PageSize, dtoList, paged.TotalItems);
        }

        private string GenerateQrCode(Guid requestId)
        {
            return $"BORROW-{requestId}";
        }

        private async Task<BorrowRequestDto> MapToDtoAsync(BorrowRequest request)
        {
            // Determine actor name: prefer account username (ProcessedByAccountId), then info fullName, then Staff navigation
            string? actorName = null;

            if (request.ProcessedByAccountId.HasValue)
            {
                try
                {
                    var account = await accountRepo.GetAccountAsync(request.ProcessedByAccountId.Value);
                    if (account != null && !string.IsNullOrWhiteSpace(account.userName))
                    {
                        actorName = account.userName; // prefer account username
                    }
                }
                catch
                {
                    // ignore and fallback
                }

                if (string.IsNullOrWhiteSpace(actorName))
                {
                    var actorInfo = await infoRepo.GetByAccountIdAsync(request.ProcessedByAccountId.Value);
                    actorName = actorInfo?.fullName;
                }
            }

            if (string.IsNullOrWhiteSpace(actorName) && request.Staff != null)
            {
                actorName = request.Staff.fullName;
            }

            return new BorrowRequestDto
            {
                Id = request.Id,
                MemberId = request.MemberId,
                MemberName = request.Member?.fullName,
                MemberEmail = request.Member?.email,
                StaffId = request.StaffId,
                StaffName = actorName,
                BookId = request.BookId,
                BookTitle = request.Book?.Title,
                BookISBN = request.Book?.ISBN,
                BookImageUrl = request.Book?.ImgUrl,
                BookCopyId = request.BookCopyId,
                CreatedAt = request.CreatedAt,
                ConfirmedAt = request.ConfirmedAt,
                BorrowDate = request.BorrowDate,
                DueDate = request.DueDate,
                ReturnedAt = request.ReturnedAt,
                Status = request.Status.ToString(),
                QrCode = request.QrCode,
                Notes = request.Notes,
                IsOverdue = request.Status == BorrowRequestStatus.Overdue || 
                           request.Status == BorrowRequestStatus.OverdueReturned
            };
        }
    }
}

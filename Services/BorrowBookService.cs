using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Book;
using System.Net.WebSockets;

namespace LibraryManagementAPI.Services
{
    public class BorrowBookService(IBookCopyRepository bookCopyRepo,
                                   IBookTransactionRepository transactionRepo,
                                   IInfoRepository infoRepo,
                                   IUnitOfWork uow) : IBorrowBookService
    {
        public async Task<bool> BorrowBookAsync(BorrowBookDto borrow)
        {
            var memberId = borrow.MemberId;
            var staffId = borrow.StaffId;
            var bookId = borrow.BookId;
            // 1. Check exist member (these are account IDs, need to check differently)
            var member = await infoRepo.GetByAccountIdAsync(memberId);
            if (member == null || member is not MemberInfo)
                throw new Exception("Member not found.");

            // 2. Check exist staff
            var staff = await infoRepo.GetByAccountIdAsync(staffId);
            if (staff == null || (staff is not StaffInfo && staff is not AdminInfo))
                throw new Exception("Staff not found.");

            // 3. Get available book copy
            var isAvailable = await bookCopyRepo.IsBookCopyAvailable(bookId);
            if (!isAvailable)
                throw new Exception("No available book copy.");

            // 4. Begin transaction
            await uow.BeginTransactionAsync();

            try
            {
                var bookCopy = await bookCopyRepo.GetById(bookId);
                // 5. Update book copy status
                bookCopy.status = Status.Borrowed;
                await bookCopyRepo.Update(bookCopy);

                // 6. Create book transaction with Info IDs
                var bookTransaction = new BookTransaction
                {
                    id = Guid.NewGuid(),
                    copyId = bookCopy.id,
                    memberId = member.id,  // Use Info ID
                    staffId = staff.id,     // Use Info ID
                };

                await transactionRepo.Add(bookTransaction);
                // 7. Commit transaction
                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // 8. Rollback transaction on error
                await uow.RollbackAsync();
                return false;
                throw new Exception("An error occurred while borrowing the book.", ex);
            }
        }

        public Task ReturnBookAsync(int loanId)
        {
            throw new NotImplementedException();
        }
    }
}

using LibraryManagementAPI.Models.Book;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IBorrowBookService
    {
        Task<bool> BorrowBookAsync(BorrowBookDto borrow);
        Task ReturnBookAsync(int loanId);
    }
}

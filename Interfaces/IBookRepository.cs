using API.Entities;

namespace LibraryManagementAPI.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();
        Task<Book?> GetBookById(Guid id);
        Task<bool> AddBook(Book category);
        Task<int> UpdateBook(Book category);
        Task<bool> DeleteBook(Guid id);
    }
}

using API.Entities;
using API.Models;

namespace LibraryManagementAPI.Interfaces
{
    public interface IBookRepository
    {
        Task<PagedResponse<Book>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20);
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<bool> AddBookAsync(Book category);
        Task<int> UpdateBookAsync(Book category);
        Task<bool> DeleteBookAsync(Guid id);
    }
}

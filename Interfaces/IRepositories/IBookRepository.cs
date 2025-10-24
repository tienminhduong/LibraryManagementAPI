using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IBookRepository
{
    Task<PagedResponse<Book>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20);
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<bool> AddBookAsync(Book category);
    Task<int> UpdateBookAsync(Book category);
    Task<bool> DeleteBookAsync(Guid id);
}

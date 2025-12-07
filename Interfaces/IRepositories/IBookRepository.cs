using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IBookRepository
{
    Task<PagedResponse<Book>> GetAllBooksAsync(Guid? categoryId, int pageNumber = 1, int pageSize = 20);
    Task<Book?> GetBookByIdAsync(Guid id);
    Task AddBookAsync(Book book, CancellationToken ct = default);
    Task<int> UpdateBookAsync(Book category);
    Task<bool> DeleteBookAsync(Guid id);
    Task UpdateCategoryOfBookAsync(Book book, IEnumerable<BookCategory> categories);
    Task UpdateAuthorOfBookAsync(Book book, IEnumerable<Author> authors);
    Task<bool> IsBookExistsByISBNAsync(string ISBN);
    Task<bool> IsBookExistsByIdAsync(Guid id);
    Task<IEnumerable<Guid>> GetAllBookIdsAsync();
    Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds);
    Task<PagedResponse<Book>> SearchByTitleAsync(string title, int pageNumber = 1, int pageSize = 20);
    Task<PagedResponse<Book>> SearchByAuthorAsync(string author, int pageNumber, int pageSize);
    Task<bool> IsbnExistsAsync(string ISBN, CancellationToken ct = default);
}

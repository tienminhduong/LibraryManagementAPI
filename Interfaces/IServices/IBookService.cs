using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IBookService
{
    // Book categories
    Task<IEnumerable<BookCategoryDto>> GetAllBookCategoriesAsync();
    Task<BookCategoryDto> GetBookCategoryByIdAsync(Guid id);
    Task<PagedResponse<BookDto>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20);
    Task<BookCategoryDto> CreateBookCategoryAsync(CreateBookCategoryDto categoryDto);
    Task UpdateBookCategoryAsync(Guid id, UpdateBookCategoryDto categoryDto);
    Task DeleteBookCategoryAsync(Guid id);
    Task<PagedResponse<BookCategoryDto>> SearchBookCategories(string query, int pageNumber = 1, int pageSize = 20);

    // Books
    Task<PagedResponse<BookDto>> GetAllBooksAsync(Guid? categoryId, int pageNumber = 1, int pageSize = 20);
    Task<BookDto?> GetBookByIdAsync(Guid id);
    Task<BookDto> AddBookAsync(CreateBookDto bookDto);
    Task UpdateCategoryOfBookAsync(Guid id, UpdateCategoryOfBookDto dto);
    Task UpdateAuthorOfBookAsync(Guid id, UpdateAuthorOfBookDto dto);
    Task<PagedResponse<BookDto>> SearchByTitleAsync(string title, int pageNumber = 1, int pageSize = 20);
    Task<PagedResponse<BookDto>> SearchByAuthorAsync(string authorName, int pageNumber = 1, int pageSize = 20);
    Task<PagedResponse<BookDto>> SearchBooks(
        string? isbn = null,
        string? titleQuery = null,
        string? categoryName = null,
        string? authorName = null,
        string? publisherName = null,
        int? publishedYear = null,
        string? descriptionContains = null,
        int pageNumber = 1,
        int pageSize = 20);

    // Book copy
    Task<Guid> ImportBooks(BookImportDto bookImportDto, Guid staffId);
}
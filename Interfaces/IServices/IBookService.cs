using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IBookService
{
    // Book categories
    Task<PagedResponse<BookCategoryDto>> GetAllBookCategoriesAsync(int pageNumber = 1, int pageSize = 20);
    Task<BookCategoryDto> GetBookCategoryByIdAsync(Guid id);
    Task<PagedResponse<BookDto>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20);
    Task<BookCategoryDto> CreateBookCategoryAsync(CreateBookCategoryDto categoryDto);
    Task UpdateBookCategoryAsync(Guid id, UpdateBookCategoryDto categoryDto);
    Task DeleteBookCategoryAsync(Guid id);
    Task<PagedResponse<BookCategoryDto>> SearchBookCategories(string? query, int pageNumber = 1, int pageSize = 20);
    Task<Response<PagedResponse<CategoryBorrowStatDto>>> GetTopCategoryByTimeAsync(int pageNumber = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null);

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
    Task<PagedResponse<BookBorrowStatDto>> GetTopBookByTimeAsync(DateTime? from = null, 
                                                                        DateTime? to = null, 
                                                                        int pageNumber = 1, 
                                                                        int pageSize = 20);

    // Book import
    Task<Guid> ImportBooks(CreateBookImportDto createBookImportDto, Guid staffId);
    Task<PagedResponse<BookImportDto>> GetImportHistoryAsync(string? supplierName = null, string? staffName = null,
        DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20);
    Task<DetailBookImportDto?> GetImportHistoryByIdAsync(Guid id);
    // Book copy
    Task<IEnumerable<BookCopy>> GetCopiesByBookIdAsync(Guid bookId);
}
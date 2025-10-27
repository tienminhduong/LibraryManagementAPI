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

    // Books
    Task<PagedResponse<BookDto>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20);
    Task<BookDto?> GetBookByIdAsync(Guid id);
    Task<BookDto> AddBookAsync(CreateBookDto bookDto);
    Task UpdateCategoryOfBookAsync(Guid id, UpdateCategoryOfBookDto dto);
    Task UpdateAuthorOfBookAsync(Guid id, UpdateAuthorOfBookDto dto);

    // Book copy
}
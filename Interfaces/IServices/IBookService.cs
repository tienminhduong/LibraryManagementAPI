using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IBookService
{
    // Book categories
    Task<IEnumerable<BookCategoryDto>> GetAllBookCategoriesAsync();
    Task<BookCategoryDto> GetBookCategoryByIdAsync(Guid id);
    Task<PagedResponse<BookDTO>> GetAllBooksInCategoryAsync(Guid id, int pageNumber = 1, int pageSize = 20);
    Task<BookCategoryDto> CreateBookCategoryAsync(CreateBookCategoryDto categoryDto);
    Task UpdateBookCategoryAsync(Guid id, UpdateBookCategoryDto categoryDto);
    Task DeleteBookCategoryAsync(Guid id);

    // Books
    Task<PagedResponse<BookDTO>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20);
    Task<BookDTO?> GetBookByIdAsync(Guid id);
    Task<BookDTO> AddBookAsync(CreateBookDTO bookDto);

    // Book copy
}
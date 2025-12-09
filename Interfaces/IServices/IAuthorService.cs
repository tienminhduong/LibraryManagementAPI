using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IAuthorService
{
    Task<PagedResponse<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize);
    Task<AuthorDto> GetAuthorAsync(Guid id);
    Task<AuthorDto> AddNewAuthor(CreateAuthorDto authorDto);
    Task UpdateAuthorAsync(Guid id, UpdateAuthorDto authorDto);
    Task DeleteAuthorAsync(Guid id);
    Task<PagedResponse<BookDto>> GetAllBooksByAuthorAsync(Guid authorId, int pageNumber, int pageSize);
    Task<PagedResponse<AuthorDto>> SearchAuthor(
        string? nameQuery = null,
        int? yearOfBirth = null,
        int? yearOfBirthBefore = null,
        int? yearOfBirthAfter = null,
        string? briefDescriptionContains = null,
        int pageNumber = 1,
        int pageSize = 20);
}
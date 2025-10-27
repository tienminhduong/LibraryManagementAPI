using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IAuthorService
{
    Task<PagedResponse<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize);
    Task<AuthorDto> GetAuthorAsync(Guid id);
    Task<AuthorDto> AddNewAuthor(CreateAuthorDto authorDto);
    Task UpdateAuthorAsync(Guid id, UpdateAuthorDto authorDto);
    Task DeleteAuthorAsync(Guid id);
}
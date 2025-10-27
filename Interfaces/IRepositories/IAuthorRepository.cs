using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IAuthorRepository
{
    Task<PagedResponse<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize);
    Task<Author> GetAuthorAsync(Guid id);
    Task UpdateAuthorAsync(Author author);
    Task DeleteAuthorAsync(Guid id);
    Task AddAuthorAsync(Author author);
}
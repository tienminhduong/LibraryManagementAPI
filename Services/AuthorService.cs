using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Services;

public class AuthorService(
    IAuthorRepository authorRepository,
    IBookRepository bookRepository,
    IMapper mapper
    ) : IAuthorService
{
    public async Task<AuthorDto> AddNewAuthor(CreateAuthorDto authorDto)
    {
        var author = mapper.Map<Author>(authorDto);
        await authorRepository.AddAuthorAsync(author);

        return mapper.Map<AuthorDto>(author);
    }

    public Task DeleteAuthorAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize)
    {
        var authors = await authorRepository.GetAllAuthorsAsync(pageNumber, pageSize);
        var authorDtos = PagedResponse<AuthorDto>.MapFrom(authors, mapper);

        return authorDtos;
    }

    public async Task<AuthorDto> GetAuthorAsync(Guid id)
    {
        var author = await authorRepository.GetAuthorAsync(id);
        return mapper.Map<AuthorDto>(author);
    }

    public Task UpdateAuthorAsync(Guid id, UpdateAuthorDto authorDto)
    {
        throw new NotImplementedException();
    }
}
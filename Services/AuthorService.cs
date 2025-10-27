using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Services;

public class AuthorService(
    IAuthorRepository authorRepository,
    IMapper mapper
    ) : IAuthorService
{
    public async Task<AuthorDto> AddNewAuthor(CreateAuthorDto authorDto)
    {
        var author = mapper.Map<Author>(authorDto);
        await authorRepository.AddAuthorAsync(author);

        return mapper.Map<AuthorDto>(author);
    }

    public async Task DeleteAuthorAsync(Guid id)
    {
        await authorRepository.DeleteAuthorAsync(id);
    }

    public async Task<PagedResponse<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize)
    {
        var authors = await authorRepository.GetAllAuthorsAsync(pageNumber, pageSize);
        var authorDtos = PagedResponse<AuthorDto>.MapFrom(authors, mapper);

        return authorDtos;
    }

    public async Task<PagedResponse<BookDto>> GetAllBooksByAuthorAsync(Guid authorId, int pageNumber, int pageSize)
    {
        var author = await authorRepository.GetAuthorAsync(authorId)
            ?? throw new NotFoundException(nameof(Author), authorId);

        var books = await authorRepository.FindBooksByAuthorAsync(authorId, pageNumber, pageSize);
        return PagedResponse<BookDto>.MapFrom(books, mapper);
    }

    public async Task<AuthorDto> GetAuthorAsync(Guid id)
    {
        var author = await authorRepository.GetAuthorAsync(id);
        return mapper.Map<AuthorDto>(author);
    }

    public async Task UpdateAuthorAsync(Guid id, UpdateAuthorDto authorDto)
    {
        var author = await authorRepository.GetAuthorAsync(id)
            ?? throw new NotFoundException(nameof(Author), id);

        author.Name = authorDto.Name;
        author.YearOfBirth = authorDto.YearOfBirth;
        author.BriefDescription = authorDto.BriefDescription;

        await authorRepository.UpdateAuthorAsync(author);
    }
}
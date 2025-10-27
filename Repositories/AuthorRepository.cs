using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories;

public class AuthorRepository(LibraryDbContext dbContext) : IAuthorRepository
{
    public async Task AddAuthorAsync(Author author)
    {
        await dbContext.Authors.AddAsync(author);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(Guid id)
    {
        var author = await dbContext.Authors.FindAsync(id)
            ?? throw new NotFoundException(nameof(Author), id);

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize)
    {
        var query = dbContext.Authors
            .AsNoTracking()
            .AsQueryable();
        return await PagedResponse<Author>.FromQueryable(query, pageNumber, pageSize);
    }

    public async Task<Author> GetAuthorAsync(Guid id)
    {
        var author = await dbContext.Authors.FindAsync(id)
            ?? throw new NotFoundException(nameof(Author), id);

        return author;
    }

    public async Task UpdateAuthorAsync(Author author)
    {
        dbContext.Update(author);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<Book>> FindBooksByAuthorAsync(Guid id, int pageNumber, int pageSize)
    {
        var query = dbContext.Books
            .Include(b => b.Authors)
            .Where(b => b.Authors.Any(a => a.Id == id));

        var books = await PagedResponse<Book>.FromQueryable(query, pageNumber, pageSize);
        return books;
    }
}
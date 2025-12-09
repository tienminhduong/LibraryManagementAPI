using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Extensions;
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
            .Include(b => b.BookCategories)
            .Where(b => b.Authors.Any(a => a.Id == id));

        var books = await PagedResponse<Book>.FromQueryable(query, pageNumber, pageSize);
        return books;
    }

    public async Task<IEnumerable<Author>> IdListToEntity(IEnumerable<Guid> authorIds)
    {
        var categories = await dbContext.Authors
            .Where(c => authorIds.Contains(c.Id))
            .ToListAsync();

        return categories;
    }

    public async Task<PagedResponse<Author>> SearchAuthor(string? nameQuery = null, int? yearOfBirth = null, int? yearOfBirthBefore = null, int? yearOfBirthAfter = null, string? briefDescriptionContains = null, int pageNumber = 1, int pageSize = 20)
    {
        var query = dbContext.Authors.AsQueryable();

        if (nameQuery != null)
            query = query.Where(a => a.Name.ToLower().Contains(nameQuery.ToLower()));

        if (yearOfBirth != null)
            query = query.Where(a => a.YearOfBirth == yearOfBirth);

        if (yearOfBirthBefore != null)
            query = query.Where(a => a.YearOfBirth < yearOfBirthBefore);

        if (yearOfBirthAfter != null)
            query = query.Where(a => a.YearOfBirth > yearOfBirthAfter);

        if (briefDescriptionContains != null)
            query = query.Where(a => a.BriefDescription == null || a.BriefDescription.ToLower().Contains(briefDescriptionContains.ToLower()));

        return await PagedResponse<Author>.FromQueryable(query, pageNumber, pageSize);
    }
}
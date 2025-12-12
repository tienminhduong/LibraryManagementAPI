using Microsoft.EntityFrameworkCore;
using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Repositories;

public class BookCategoryRepository(LibraryDbContext dbContext) : IBookCategoryRepository
{
    public async Task<PagedResponse<BookCategory>> GetAllCategories(int pageNumber = 1, int pageSize = 20)
    {
        var query = dbContext.BookCategories.AsQueryable();
        return await PagedResponse<BookCategory>.FromQueryable(query, pageNumber, pageSize);
    }

    public async Task<BookCategory?> GetCategoryByIdAsync(Guid id)
    {
        return await dbContext.BookCategories.FindAsync(id);
    }

    public async Task<bool> AddCategoryAsync(BookCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        await dbContext.BookCategories.AddAsync(category);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCategory(BookCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        dbContext.BookCategories.Update(category);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategory(Guid id)
    {
        var category = await dbContext.BookCategories.FindAsync(id);
        if (category == null) return false;

        dbContext.BookCategories.Remove(category);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsCategoryExistsByName(string name)
    {
        return await dbContext.BookCategories
            .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower()) != null;
    }

    public async Task<int> CountBooksByCategory(Guid categoryId)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Include(b => b.BookCategories)
                .ThenInclude(c => c.Books)
            .CountAsync(b => b.BookCategories.Any(c => c.Id == categoryId));
    }

    public async Task<PagedResponse<Book>> SearchBookByCategory(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        var query = dbContext.Books
            .Include(b => b.BookCategories)
            .Where(b => b.BookCategories.Any(c => c.Id == id));

        var books = await PagedResponse<Book>.FromQueryable(query, pageNumber, pageSize);
        return books;
    }

    public async Task<IEnumerable<BookCategory>> IdListToEntity(IEnumerable<Guid> ids)
    {
        var categories = await dbContext.BookCategories
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();

        return categories;
    }

    public async Task<PagedResponse<BookCategory>> GetBookCategoriesByName(string? query = null, int pageNumber = 1, int pageSize = 20)
    {
        var dbQuery = dbContext.BookCategories.AsQueryable();
        if (query != null)
            dbQuery = dbQuery.Where(c => c.Name.ToLower().Contains(query.ToLower()));

        var categories = await PagedResponse<BookCategory>.FromQueryable(dbQuery, pageNumber, pageSize);

        return categories;
    }
}
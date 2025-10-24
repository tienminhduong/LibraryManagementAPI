using Microsoft.EntityFrameworkCore;
using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;

namespace LibraryManagementAPI.Repositories;

public class BookCategoryRepository(LibraryDbContext dbContext) : IBookCategoryRepository
{
    public async Task<IEnumerable<BookCategory>> GetAllCategories()
    {
        return await dbContext.BookCategories.ToListAsync();
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
}
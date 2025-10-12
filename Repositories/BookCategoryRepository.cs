using API.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class BookCategoryRepository(LibraryDbContext dbContext) : IBookCategoryRepository
{
    public async Task<IEnumerable<BookCategory>> GetAllCategories()
    {
        return await dbContext.BookCategories.ToListAsync();
    }
    public async Task<BookCategory?> GetCategoryById(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Invalid category ID", nameof(id));
        }
        return await dbContext.BookCategories.FindAsync(id);
    }
    public async Task<bool> AddCategory(BookCategory category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }
        await dbContext.BookCategories.AddAsync(category);
        return await dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateCategory(BookCategory category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }
        dbContext.BookCategories.Update(category);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategory(Guid id)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentException("Invalid category ID", nameof(id));
        }
        var category = await dbContext.BookCategories.FindAsync(id);
        if (category == null) return false;
        dbContext.BookCategories.Remove(category);
        return await dbContext.SaveChangesAsync() > 0;
    }
}
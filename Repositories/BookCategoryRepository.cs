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
}
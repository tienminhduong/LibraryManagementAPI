using Microsoft.EntityFrameworkCore;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<BookCategory> BookCategories { get; set; }
}
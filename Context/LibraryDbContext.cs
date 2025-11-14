using LibraryManagementAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Context;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AdminInfo> AdminInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookCategories)
            .WithMany(c => c.Books)
            .UsingEntity(j => j.ToTable("BookCategoryMap"));

        modelBuilder.Entity<AdminInfo>()
            .HasOne(info => info.LoginInfoId)
            .WithOne(login => login.AdminInfo);
    }
}

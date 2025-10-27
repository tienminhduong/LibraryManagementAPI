using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories;

public class BookRepository(LibraryDbContext dbContext) : IBookRepository
{
    public async Task<bool> AddBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await dbContext.Books.AddAsync(book);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var book = await dbContext.Books.FindAsync(id);

        if (book == null)
            return false;

        dbContext.Books.Remove(book);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<PagedResponse<Book>> GetAllBooksAsync(int pageNumber = 1, int pageSize = 20)
    {
        var books = dbContext.Books
            .Include(b => b.BookCategories)
            .Include(b => b.Authors);
        return await PagedResponse<Book>.FromQueryable(books, pageNumber, pageSize);
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await dbContext.Books
            .Include(b => b.BookCategories)
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(book => book.Id == id);
    }

    public async Task<bool> IsBookExistsByISBNAsync(string ISBN)
    {
        return await dbContext.Books
            .FirstOrDefaultAsync(b => b.ISBN == ISBN) != null;
    }

    public async Task UpdateAuthorOfBookAsync(Book book, IEnumerable<Author> authors)
    {
        book.Authors = [.. authors];
        dbContext.Books.Update(book);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateBookAsync(Book book)
    {
        var isExisting = await IsBookExistsByISBNAsync(book.ISBN);
        if (!isExisting)
            throw new ArgumentNullException(nameof(book), "Book is not existing");

        dbContext.Books.Update(book);
        return await dbContext.SaveChangesAsync();
    }

    public async Task UpdateCategoryOfBookAsync(Book book, IEnumerable<BookCategory> categories)
    {
        book.BookCategories = [.. categories];
        dbContext.Books.Update(book);
        await dbContext.SaveChangesAsync();
    }
}

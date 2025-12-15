using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

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

    public async Task<PagedResponse<Book>> GetAllBooksAsync(Guid? categoryId, int pageNumber = 1, int pageSize = 20)
    {
        var books = dbContext.Books
                    .Where(b => categoryId == null || b.BookCategories.Any(c => c.Id == categoryId))
                    .OrderBy(b => b.Title)               // Phân trang cần OrderBy
                    .Include(b => b.BookCategories)      // Include sau khi lọc
                    .Include(b => b.Authors)
                    .AsSplitQuery();
        return await PagedResponse<Book>.FromQueryable(books, pageNumber, pageSize);
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await dbContext.Books
            .Include(b => b.BookCategories)
            .Include(b => b.Authors)
            .AsSplitQuery()
            .FirstOrDefaultAsync(book => book.Id == id);
    }

    public async Task<bool> IsBookExistsByISBNAsync(string ISBN)
    {
        return await dbContext.Books
            .FirstOrDefaultAsync(b => b.ISBN == ISBN) != null;
    }

    public async Task<bool> IsBookExistsByIdAsync(Guid id)
    {
        return await dbContext.Books
            .AnyAsync(b => b.Id == id);
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

    public async Task<IEnumerable<Guid>> GetAllBookIdsAsync()
    {
        return await dbContext.Books
            .Select(b => b.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
    {
        return await dbContext.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToListAsync();
    }

    public async Task<PagedResponse<Book>> SearchByTitleAsync(string title, int pageNumber = 1, int pageSize = 20)
    {
        // create query
        var query = dbContext.Books
            .OrderBy(b => b.Title)
            .AsNoTracking()
            .AsSplitQuery() // Tách thành 2 queries: 1 cho Books, 1 cho Authors
            .Include(b => b.Authors) // Join với Authors qua bookAuthor
            .Where(b => EF.Functions.Like(b.Title, $"%{title}%"));

        // get paged result
        var pagedResult = await PagedResponse<Book>.FromQueryable(query, pageNumber, pageSize);
        return pagedResult;
    }

    public Task<PagedResponse<Book>> SearchByAuthorAsync(string authorName, int pageNumber, int pageSize)
    {
        // create query
        var query = dbContext.Books
            .OrderBy(b => b.Title)
            .AsNoTracking()
            .AsSplitQuery() // Tối ưu cho Many-to-Many
            .Include(b => b.Authors) // Join Authors
            .Where(b => b.Authors.Any(a => EF.Functions.Like(a.Name, $"%{authorName}%")));

        // get paged result
        return PagedResponse<Book>.FromQueryable(query, pageNumber, pageSize);
    }

    public async Task<PagedResponse<Book>> SearchBooks(
        string? isbn = null,
        string? titleQuery = null,
        string? categoryName = null,
        string? authorName = null,
        string? publisherName = null,
        int? publishedYear = null,
        string? descriptionContains = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var books = dbContext.Books
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .Include(b => b.Authors)
            .Include(b => b.BookCategories)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (isbn != null)
            books = books.Where(b => b.ISBN == isbn);
        if (titleQuery != null)
            books = books.Where(b => b.Title.ToLower().Contains(titleQuery.ToLower()));
        if (categoryName != null)
            books = books.Where(b => b.BookCategories.Any(c => c.Name.ToLower().Contains(categoryName.ToLower())));
        if (authorName != null)
            books = books
                .Where(b => b.Authors.Any(a => a.Name.ToLower().Contains(authorName.ToLower())));
        if (publisherName != null)
            books = books
                .Where(b => b.Publisher != null && b.Publisher.Name.ToLower().Contains(publisherName.ToLower()));
        if (publishedYear != null)
            books = books.Where(b => b.PublicationYear == publishedYear);
        if (descriptionContains != null)
            books = books.Where(b => b.Description != null && b.Description.ToLower().Contains(descriptionContains.ToLower()));
        return await PagedResponse<Book>.FromQueryable(books, pageNumber, pageSize);
    }
}

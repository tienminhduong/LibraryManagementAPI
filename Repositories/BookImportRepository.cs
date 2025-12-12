using System.Linq.Expressions;
using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories;

public class BookImportRepository(LibraryDbContext dbContext) : IBookImportRepository
{
    public async Task AddBookImportAsync(BookImport bookImport)
    {
        await dbContext.BookImports.AddAsync(bookImport);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<BookImport>> GetImportHistoryAsync(int pageNumber, int pageSize)
    {
        var query = dbContext.BookImports
            .Include(import => import.supplier)
            .Include(import => import.staff)
            .Include(import => import.BookImportDetails)!
                .ThenInclude(details => details.book)
            .AsQueryable();
        return await  PagedResponse<BookImport>.FromQueryable(query, pageNumber, pageSize);
    }

    public async Task AddImportDetailAsync(BookImportDetail bookImportDetail)
    {
        await dbContext.BookImportDetails.AddAsync(bookImportDetail);
        await dbContext.SaveChangesAsync();
    }

    public async Task<BookImport?> GetByIdAsync(Guid id)
    {
        var bookImport = await dbContext.BookImports
            .Include(import => import.supplier)
            .Include(import => import.staff)
            .Include(import => import.BookImportDetails)!
            .ThenInclude(details => details.book).ThenInclude(books => books.Authors)
            .Include(import => import.BookImportDetails)!
            .ThenInclude(details => details.book).ThenInclude(books => books.BookCategories)
            .Include(import => import.BookImportDetails)!
            .ThenInclude(details => details.book).ThenInclude(books => books.Publisher)
            .Where(import => import.id == id)
            .FirstOrDefaultAsync();

        return bookImport;
    }

    public async Task<PagedResponse<BookImport>> GetImportHistoryAsync<TKey>(
        Expression<Func<BookImport, TKey>> keySelector,
        bool isDescending = false,
        bool withDetails = false,
        bool withBookCopyDetails = false,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var query = dbContext.BookImports.AsQueryable();

        query = !isDescending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);

        if (!withDetails) return await PagedResponse<BookImport>.FromQueryable(query, pageNumber, pageSize);
        if (withBookCopyDetails)
            query.Include(bookImport => bookImport.BookImportDetails!)
                .ThenInclude(details => details.book);
        else
            query.Include(bookImport => bookImport.BookImportDetails);

        return await PagedResponse<BookImport>.FromQueryable(query, pageNumber, pageSize);
    }

    public async Task<bool> ImportBookAsync(BookImport bookImport)
    {
        ArgumentNullException.ThrowIfNull(bookImport);
        foreach (var importDetail in bookImport.BookImportDetails ?? [])
        {
            if (importDetail.book != null)
                await dbContext.Books.AddAsync(importDetail.book);

            await dbContext.BookImportDetails.AddAsync(importDetail);
        }
        await dbContext.BookImports.AddAsync(bookImport);
        return await dbContext.SaveChangesAsync() > 0;
    }
}
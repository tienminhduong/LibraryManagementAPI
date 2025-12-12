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

    public Task<BookImport> GetByIdAsync(Guid id, bool withDetails = false, bool withBookCopyDetails = false)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<BookImport>> GetImportHistoryAsync<Tkey>(
        Expression<Func<BookImport, Tkey>> keySelector,
        bool isDescending = false,
        bool withDetails = false,
        bool withBookCopyDetails = false,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var query = dbContext.BookImports.AsQueryable();

        if (!isDescending)
            query.OrderBy(keySelector);
        else
            query.OrderByDescending(keySelector);

        if (withDetails)
        {
            if (withBookCopyDetails)
                query.Include(bookImport => bookImport.BookImportDetails!)
                    .ThenInclude(details => details.book);
            else
                query.Include(bookImport => bookImport.BookImportDetails);
        }

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
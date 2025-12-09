using System.Linq.Expressions;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IBookImportRepository
{
    Task<BookImport> GetByIdAsync(Guid id, bool withDetails = false, bool withBookCopyDetails = false);
    Task<PagedResponse<BookImport>> GetImportHistoryAsync<Tkey>(
        Expression<Func<BookImport, Tkey>> keySelector,
        bool isDescending = false,
        bool withDetails = false,
        bool withBookCopyDetails = false,
        int pageNumber = 1,
        int pageSize = 20);
    Task<bool> ImportBookAsync(BookImport bookImport);
    Task AddImportDetailAsync(BookImportDetail bookImportDetail);
    Task AddBookImportAsync(BookImport bookImport);
}
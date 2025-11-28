using LibraryManagementAPI.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementAPI.Interfaces.IUtility
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
        LibraryDbContext DbContext { get; }
    }

}

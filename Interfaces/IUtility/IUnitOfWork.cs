using LibraryManagementAPI.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementAPI.Interfaces.IUtility
{
    public interface IUnitOfWork: IDisposable
    {
        IDbContextTransaction BeginTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitAsync();
        Task RollbackAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext dbContext;
        private IDbContextTransaction? currentTransaction;
        public UnitOfWork(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IDbContextTransaction BeginTransaction()
        {
            // chi 1 transaction dc duoc tao
            if (currentTransaction != null)
            {
                return currentTransaction;
            }
            currentTransaction = dbContext.Database.BeginTransaction();
            return currentTransaction;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task CommitAsync()
        {
            if (currentTransaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }
            await currentTransaction.CommitAsync();
            await currentTransaction.DisposeAsync();
            currentTransaction = null;
        }

        public Task RollbackAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IUtility;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementAPI.Models.Utility
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _db;
        private IDbContextTransaction _transaction;

        public UnitOfWork(LibraryDbContext db)
        {
            _db = db;
        }

        public LibraryDbContext DbContext => _db;


        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
            await _transaction.CommitAsync();
        }


        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }

}

using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class BookTransactionRepository(LibraryDbContext db) : IBookTransactionRepository
    {
        public async Task Add(BookTransaction bookTransaction)
        {
            try
            {
                await db.BookTransactions.AddAsync(bookTransaction);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the book transaction.", ex);
            }
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BookTransaction>> GetAll()
        {
            try
            {
                return await db.BookTransactions
                    .Include(t => t.book)
                        .ThenInclude(bc => bc.book)
                    .Include(t => t.member)
                    .Include(t => t.staff)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving book transactions.", ex);
            }
        }

        public Task<BookTransaction> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(BookTransaction bookTransaction)
        {
            try
            {
                db.BookTransactions.Update(bookTransaction);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the book transaction.", ex);
            }
        }
    }
}

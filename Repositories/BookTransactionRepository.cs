using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IServices;

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

        public Task<IEnumerable<BookTransaction>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<BookTransaction> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(BookTransaction bookTransaction)
        {
            throw new NotImplementedException();
        }
    }
}

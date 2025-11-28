using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;

namespace LibraryManagementAPI.Repositories
{
    public class BookCopyRepository(LibraryDbContext db) : IBookCopyRepository
    {
        public Task Add(BookCopy bookCopy)
        {
            try
            {
                db.BookCopies.Add(bookCopy);
                db.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the book copy.", ex);
            }
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookCopy>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<BookCopy> GetById(Guid id)
        {
            try
            {
                var bookCopy = await db.BookCopies.FindAsync(id);
                if (bookCopy == null)
                    throw new Exception("Book copy not found.");
                return bookCopy;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while retrieving the book copy.", ex);
            }
        }

        public async Task<bool> IsBookCopyAvailable(Guid bookCopyId)
        {
            try
            {
                var bookCopy = await db.BookCopies.FindAsync(bookCopyId);

                if (bookCopy == null)
                    throw new Exception("Book copy not found.");

                return bookCopy.status == Status.Available;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking book copy availability.", ex);
            }
        }

        public Task Update(BookCopy bookCopy)
        {
            try
            {
                db.BookCopies.Update(bookCopy);
                db.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the book copy.", ex);
            }
        }
    }
}

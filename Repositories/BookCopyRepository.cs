using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> HasAvailableCopiesForBook(Guid bookId)
        {
            try
            {
                return await db.BookCopies
                    .AnyAsync(bc => bc.bookId == bookId && bc.status == Status.Available);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking book availability.", ex);
            }
        }

        public async Task<IEnumerable<BookCopy>> GetAvailableCopiesByBookId(Guid bookId)
        {
            try
            {
                return await db.BookCopies
                    .Where(bc => bc.bookId == bookId && bc.status == Status.Available)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving available book copies.", ex);
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

        /// <summary>
        /// Generates a permanent QR code for a BookCopy.
        /// Format: COPY-{BookCopyId}
        /// </summary>
        public string GenerateQrCode(Guid bookCopyId)
        {
            return $"COPY-{bookCopyId}";
        }

        /// <summary>
        /// Gets a BookCopy by its QR code.
        /// </summary>
        public async Task<BookCopy?> GetByQrCode(string qrCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(qrCode) || !qrCode.StartsWith("COPY-"))
                    return null;

                var idString = qrCode.Substring(5);
                if (!Guid.TryParse(idString, out var bookCopyId))
                    return null;

                return await db.BookCopies
                    .Include(bc => bc.book)
                    .FirstOrDefaultAsync(bc => bc.id == bookCopyId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving book copy by QR code.", ex);
            }
        }
    }
}

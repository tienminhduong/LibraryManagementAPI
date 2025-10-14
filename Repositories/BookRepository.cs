using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class BookRepository(LibraryDbContext dbContext) : Interfaces.IBookRepository
    {
        public async Task<bool> AddBook(Book book)
        {
            // check book is null
            if(book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            // save book to dbcontext
            await dbContext.Books.AddAsync(book);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBook(Guid id)
        {
            // Validate the ID
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid book ID", nameof(id));
            }
            // Find the book by ID
            var book = await dbContext.Books.FindAsync(id);
            // If the book does not exist, return false
            if (book == null)
            {
                return false;
            }
            // Remove the book from the DbContext
            dbContext.Books.Remove(book);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await dbContext.Books.Include(book => book.Category).ToListAsync();
        }

        public async Task<Book?> GetBookById(Guid id)
        {
            return await dbContext.Books.Include(book => book.Category).FirstOrDefaultAsync(book => book.Id == id);
        }

        public async Task<bool> IsBookExistsByISBN(string ISBN)
        {
            return await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == ISBN) != null;
        }

        public async Task<int> UpdateBook(Book category)
        {
            // check book is exist
            var isExisting = await IsBookExistsByISBN(category.ISBN);
            if (!isExisting)
            {
                throw new ArgumentNullException(nameof(category), "Book is not existing");
            }
            // update book in dbcontext
            dbContext.Books.Update(category);
            // return row effected
            return await dbContext.SaveChangesAsync();
            
        }
    }
}

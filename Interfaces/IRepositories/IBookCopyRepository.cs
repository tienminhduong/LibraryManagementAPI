using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IBookCopyRepository
    {
        Task<BookCopy> GetById(Guid id);
        Task<IEnumerable<BookCopy>> GetAll();
        Task Add(BookCopy bookCopy);
        Task Update(BookCopy bookCopy);
        Task Delete(Guid id);
        Task<bool> IsBookCopyAvailable(Guid bookCopyId);
        Task<bool> HasAvailableCopiesForBook(Guid bookId);
        Task<IEnumerable<BookCopy>> GetAvailableCopiesByBookId(Guid bookId);
        Task<BookCopy?> GetByQrCode(string qrCode);
        string GenerateQrCode(Guid bookCopyId);
        Task<IEnumerable<BookCopy>> GetCopiesByBookId(Guid bookId);
    }
}

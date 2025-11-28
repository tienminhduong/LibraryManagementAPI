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
    }
}

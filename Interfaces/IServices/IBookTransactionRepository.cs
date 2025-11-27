using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IBookTransactionRepository
    {
        Task<BookTransaction> GetById(Guid id);
        Task<IEnumerable<BookTransaction>> GetAll();
        Task Add(BookTransaction bookTransaction);
        Task Update(BookTransaction bookTransaction);
        Task Delete(Guid id);
    }
}

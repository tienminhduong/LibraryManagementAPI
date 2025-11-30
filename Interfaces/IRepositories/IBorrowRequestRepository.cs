using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IBorrowRequestRepository
    {
        Task<BorrowRequest?> GetById(Guid id);
        Task<BorrowRequest?> GetByIdWithDetails(Guid id);
        Task<IEnumerable<BorrowRequest>> GetAll();
        Task<IEnumerable<BorrowRequest>> GetByMemberId(Guid memberId);
        Task<IEnumerable<BorrowRequest>> GetByStatus(BorrowRequestStatus status);
        Task Add(BorrowRequest borrowRequest);
        Task Update(BorrowRequest borrowRequest);
        Task Delete(Guid id);
    }
}

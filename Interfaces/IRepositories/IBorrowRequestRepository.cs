using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.User;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IBorrowRequestRepository
    {
        Task<BorrowRequest?> GetById(Guid id);
        Task<BorrowRequest?> GetByIdWithDetails(Guid id);
        Task<IEnumerable<BorrowRequest>> GetAll();
        Task<IEnumerable<BorrowRequest>> GetByMemberId(Guid memberId);
        Task<PagedResponse<BorrowRequest>> GetByMemberIdPaged(Guid memberId, int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequest>> GetByMemberIdAndStatusPaged(Guid memberId, BorrowRequestStatus? status, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<BorrowRequest>> GetByStatus(BorrowRequestStatus status);
        Task<PagedResponse<BorrowRequest>> GetByStatusPaged(BorrowRequestStatus status, int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequest>> GetAllByStatusPaged(BorrowRequestStatus? status, int pageNumber = 1, int pageSize = 20);
        Task Add(BorrowRequest borrowRequest);
        Task Update(BorrowRequest borrowRequest);
        Task Delete(Guid id);
        Task<BorrowRequest?> GetByBookCopyIdAsync(Guid bookCopyId);
        Task<BorrowRequest?> GetByQrCodeAsync(string qrCode);
        Task<PagedResponse<LateReturnedUserDto>> GetBorrowCountForMemberAsync(int pageNumber = 1,
            int pageSize = 20);
    }
}

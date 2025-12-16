using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.User;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IBorrowRequestService
    {
        Task<BorrowRequestResponseDto> CreateBorrowRequestAsync(CreateBorrowRequestDto dto, Guid accountId);
        Task<BorrowRequestResponseDto> AdminCreateBorrowRequestAsync(AdminCreateBorrowRequestDto dto, Guid staffAccountId);
        Task<IEnumerable<MemberSearchDto>> SearchMembersAsync(string searchTerm);
        Task<BorrowRequestDto?> GetBorrowRequestByIdAsync(Guid id, Guid accountId, string userRole);
        Task<BorrowRequestDto?> GetBorrowRequestByQrCodeAsync(string qrCode);
        Task<bool> ConfirmBorrowRequestAsync(ConfirmBorrowRequestDto dto, Guid staffAccountId);
        Task<bool> RejectBorrowRequestAsync(Guid requestId, Guid staffAccountId, string reason);
        Task<bool> CancelBorrowRequestAsync(Guid requestId, Guid memberAccountId);
        
        // New unified methods with optional status filtering
        Task<PagedResponse<BorrowRequestDto>> GetBorrowRequestsAsync(BorrowRequestStatusFilter? status, int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetMemberBorrowRequestsAsync(Guid memberInfoId, BorrowRequestStatusFilter? status, int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetMyBorrowRequestsAsync(Guid accountId, BorrowRequestStatusFilter? status, int pageNumber = 1, int pageSize = 20);
        
        // Legacy methods (deprecated, kept for backward compatibility)
        [Obsolete("Use GetBorrowRequestsAsync with status parameter instead")]
        Task<PagedResponse<BorrowRequestDto>> GetPendingRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetMyBorrowRequestsAsync instead")]
        Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsPagedAsync(Guid memberAccountId, int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetMemberBorrowRequestsAsync instead")]
        Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsByInfoIdPagedAsync(Guid memberInfoId, int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetBorrowRequestsAsync with status parameter instead")]
        Task<PagedResponse<BorrowRequestDto>> GetBorrowedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetBorrowRequestsAsync with status parameter instead")]
        Task<PagedResponse<BorrowRequestDto>> GetOverdueRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetBorrowRequestsAsync with status parameter instead")]
        Task<PagedResponse<BorrowRequestDto>> GetReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        [Obsolete("Use GetBorrowRequestsAsync with status parameter instead")]
        Task<PagedResponse<BorrowRequestDto>> GetOverdueReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        
        Task<ReturnBookResultDto> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId);
    }
}

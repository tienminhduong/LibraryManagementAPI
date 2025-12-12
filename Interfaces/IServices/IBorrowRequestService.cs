using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Pagination;

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
        Task<PagedResponse<BorrowRequestDto>> GetPendingRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsPagedAsync(Guid memberAccountId, int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetMemberRequestsByInfoIdPagedAsync(Guid memberInfoId, int pageNumber = 1, int pageSize = 20);
        Task<ReturnBookResultDto> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId);
        Task<PagedResponse<BorrowRequestDto>> GetBorrowedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetOverdueRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
        Task<PagedResponse<BorrowRequestDto>> GetOverdueReturnedRequestsPagedAsync(int pageNumber = 1, int pageSize = 20);
    }
}

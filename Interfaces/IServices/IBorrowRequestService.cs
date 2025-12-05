using LibraryManagementAPI.Models.BorrowRequest;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IBorrowRequestService
    {
        Task<BorrowRequestResponseDto> CreateBorrowRequestAsync(CreateBorrowRequestDto dto, Guid accountId);
        Task<BorrowRequestDto?> GetBorrowRequestByIdAsync(Guid id, Guid accountId, string userRole);
        Task<BorrowRequestDto?> GetBorrowRequestByQrCodeAsync(string qrCode);
        Task<bool> ConfirmBorrowRequestAsync(ConfirmBorrowRequestDto dto, Guid staffAccountId);
        Task<bool> RejectBorrowRequestAsync(Guid requestId, Guid staffAccountId, string reason);
        Task<bool> CancelBorrowRequestAsync(Guid requestId, Guid memberAccountId);
        Task<IEnumerable<BorrowRequestDto>> GetPendingRequestsAsync();
        Task<IEnumerable<BorrowRequestDto>> GetMemberRequestsAsync(Guid memberAccountId);
        Task<IEnumerable<BorrowRequestDto>> GetMemberRequestsByInfoIdAsync(Guid memberInfoId);
        Task<bool> ReturnBookAsync(ReturnBookDto dto, Guid staffAccountId);
    }
}

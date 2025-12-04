using LibraryManagementAPI.Models.BorrowRequest;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IBorrowRequestService
    {
        Task<BorrowRequestResponseDto> CreateBorrowRequestAsync(CreateBorrowRequestDto dto);
        Task<BorrowRequestDto?> GetBorrowRequestByIdAsync(Guid id);
        Task<BorrowRequestDto?> GetBorrowRequestByQrCodeAsync(string qrCode);
        Task<bool> ConfirmBorrowRequestAsync(ConfirmBorrowRequestDto dto);
        Task<bool> RejectBorrowRequestAsync(Guid requestId, Guid staffId, string reason);
        Task<bool> CancelBorrowRequestAsync(Guid requestId, Guid memberId);
        Task<IEnumerable<BorrowRequestDto>> GetPendingRequestsAsync();
        Task<IEnumerable<BorrowRequestDto>> GetMemberRequestsAsync(Guid memberId);
        Task<bool> ReturnBookAsync(ReturnBookDto dto);
    }
}

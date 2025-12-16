using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Repositories;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Response<string>> Login(string userName, string password);
        Task<Response<string>> Register(CreateAccountDto createAccountDto);
        Task<Response<string>> ResetPassword(string userEmail, string newPassword);
        Task<bool> BanAccount(Guid accountId);
        Task<PagedResponse<InFoAccountBorrow>> GetInfoAccountBorrow(string? keyword, int pageNumber = 1,
            int pageSize = 20);
    }
}

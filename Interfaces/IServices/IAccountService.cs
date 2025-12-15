using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Response<string>> Login(string userName, string password);
        Task<Response<string>> Register(CreateAccountDto createAccountDto);
        Task<Response<string>> ResetPassword(string userEmail, string newPassword);
        Task<bool> BanAccount(Guid accountId);
    }
}

using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Response<string>> Login(string userName, string password);
        Task<Response<bool>> Register(CreateAccountDto createAccountDto);
    }
}

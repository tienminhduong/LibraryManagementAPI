using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<bool> Login(string userName, string password);
        Task<AccountDto> Register(CreateAccountDto createAccountDto);
    }
}

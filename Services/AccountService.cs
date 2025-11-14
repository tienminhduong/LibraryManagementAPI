using AutoMapper;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Account;

namespace LibraryManagementAPI.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IMapper mapper,
        IHasherPassword hasher
        ) : IAccountService
    {
        public Task<bool> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<AccountDto> Register(CreateAccountDto createAccountDto)
        {
            throw new NotImplementedException();
        }

        //public async Task<AccountDto> Register(CreateAccountDto createAccountDto)
        //{
        //    using (var transaction = await )
        //}
    }
}

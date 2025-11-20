using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Account;

namespace LibraryManagementAPI.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IInfoRepository infoRepository,
        IMapper mapper,
        IHasherPassword hasher
        ) : IAccountService
    {
        public Task<bool> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Register(CreateAccountDto createAccountDto)
        {
            try
            {
                var account = new Account
                {
                    userName = createAccountDto.userName,
                    passwordHash = hasher.HashPassword(createAccountDto.password),
                    role = createAccountDto.role,
                };
                var info = mapper.Map<BaseInfo>(createAccountDto.adminInfo);
                accountRepository.AddAccountAsync(account);
                infoRepository.AddAsync(info);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering the account.", ex);
            }
        }

        //public async Task<AccountDto> Register(CreateAccountDto createAccountDto)
        //{
        //    using (var transaction = await )
        //}
    }
}

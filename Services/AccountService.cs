using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Info;

namespace LibraryManagementAPI.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IHasherPassword hasher
        ) : IAccountService
    {
        public Task<bool> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Register(CreateAccountDto createAccountDto)
        {
            var db = accountRepository.GetDbContext();
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var account = new Account
                {
                    userName = createAccountDto.userName,
                    passwordHash = hasher.HashPassword(createAccountDto.password),
                    role = createAccountDto.role
                };
                
                var infoEntity = MapInfoDtoToEntity(createAccountDto.info, createAccountDto.role);

                await accountRepository.AddAccountAsync(account, infoEntity);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while registering the account.", ex);
            }
        }

        private BaseInfo MapInfoDtoToEntity(BaseInfoDto infoDto, Role role)
        {
            BaseInfo infoEntity = null;
            if(CheckRoleWithType(infoDto, role) == false)
            {
                throw new ArgumentException("Info DTO type does not match the specified role.");
            }
            switch (role)
            {
                case Role.Admin:
                    infoEntity = new AdminInfo()
                    {
                        fullName = infoDto.fullName,
                        email = infoDto.email,
                        phoneNumber = infoDto.phoneNumber
                    };
                    break;
                case Role.Staff:
                    infoEntity = new StaffInfo()
                    {
                        fullName = infoDto.fullName,
                        email = infoDto.email,
                        phoneNumber = infoDto.phoneNumber,
                        hireDate = (infoDto as StaffInfoDto)?.hireDate ?? DateTime.UtcNow
                    };
                    break;
                case Role.Member:
                    infoEntity = new MemberInfo()
                    {
                        fullName = infoDto.fullName,
                        email = infoDto.email,
                        phoneNumber = infoDto.phoneNumber,
                        address = (infoDto as MemberInfoDto)?.address,
                        imageUrl = (infoDto as MemberInfoDto)?.imageUrl,
                        joinDate = (infoDto as MemberInfoDto)?.joinDate ?? DateTime.UtcNow
                    };
                    break;
                default:
                    throw new ArgumentException("Invalid role for info mapping.");
            }
            return infoEntity;
        }

        private bool CheckRoleWithType(BaseInfoDto info, Role role)
        {
            return (role == Role.Admin && info is AdminInfoDto) ||
                   (role == Role.Staff && info is StaffInfoDto) ||
                   (role == Role.Member && info is MemberInfoDto);
        }
        //public async Task<AccountDto> Register(CreateAccountDto createAccountDto)
        //{
        //    using (var transaction = await )
        //}
    }
}

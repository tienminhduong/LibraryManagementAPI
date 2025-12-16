using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Account;
using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IHasherPassword hasher,
        ITokenService jwtTokenService,
        IInfoRepository infoRepository
        ) : IAccountService
    {
        public async Task<Response<string>> Login(string userName, string password)
        {
            try
            {
                var account = await accountRepository.GetAccountAsync(userName);

                // not found account
                if (account == null)
                {
                    return Response<string>.Failure("Account not found.");
                }
                
                // invalid password
                var isPasswordValid = hasher.VerifyPassword(password, account.passwordHash);
                if (!isPasswordValid)
                {
                    return Response<string>.Failure("Invalid username or password.");
                }

                // check if account is active
                if (!account.isActive)
                {
                    return Response<string>.Failure("Account is banned.");
                }

                // get info
                var info = await infoRepository.GetInfoByAccountIdAsync(account.id, account.role);
                if (info == null)
                {
                    return Response<string>.Failure("Associated user info not found.");
                }
                // success
                var token = jwtTokenService.GenerateToken(account, info);
                return Response<string>.Success(token);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while logging in.", ex);
            }
        }

        public async Task<Response<string>> Register(CreateAccountDto createAccountDto)
        {
            var db = accountRepository.GetDbContext();

            var existingUserName = db.Accounts.Any(a => a.userName == createAccountDto.userName);
            if (existingUserName)
            {
                return Response<string>.Failure("Username already exists.");
            }

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
                return Response<string>.Success("Register Success");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("Registration failed: " + ex.Message);
                //throw new Exception("An error occurred while registering the account.", ex);
            }
        }

        public async Task<Response<string>> ResetPassword(string userEmail, string newPassword)
        {
            var dbContext = accountRepository.GetDbContext();
            var account = dbContext.Accounts.Include(a => a.info).FirstOrDefault(a => a.info!.email == userEmail);
            
            if (account == null)
            {
                return Response<string>.Failure("Account not found.");
            }
            
            account.passwordHash = hasher.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();
            return Response<string>.Success("Password reset successful.");
        }
        
        public async Task<bool> BanAccount(Guid accountId)
        {
            return await accountRepository.UpdateAccountStatus(accountId, false);
        }

        private BaseInfo? MapInfoDtoToEntity(BaseInfoDto infoDto, Role role)
        {
            BaseInfo? infoEntity = null;
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

        public async Task<PagedResponse<InFoAccountBorrow>> GetInfoAccountBorrow(string? keyword,int pageNumber = 1,
            int pageSize = 20)
        {
            var res = await accountRepository.GetInfoBorrowForMemberAccountAsync(keyword, pageNumber, pageSize);
            return res;
        }
    }
}

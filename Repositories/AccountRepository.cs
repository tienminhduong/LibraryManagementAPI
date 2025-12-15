using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Repositories
{
    public class AccountRepository(LibraryDbContext db, IHasherPassword hasherPassword) : IAccountRepository
    {
        public async Task AddAccountAsync(Account loginInfo, BaseInfo info)
        {
            try
            {
                // them account truoc
                await db.Accounts.AddAsync(loginInfo); 
                await db.SaveChangesAsync();

                // gan id account vao info
                info.loginId = loginInfo.id;
                // fix lai ngay vi posgres chi chap nhan DateTimeKind.Utc
                info.ConvertTimezone();

                // them info sau
                await db.AddAsync(info);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the login info.", ex);
            }
        }

        public Task DeleteAccountAsync(Guid id)
        {
            try
            {
                var loginInfo = db.Accounts.Find(id);
                if (loginInfo != null)
                {
                    db.Accounts.Remove(loginInfo);
                    db.SaveChangesAsync();
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the login info.", ex);
            }
        }

        public Task<PagedResponse<Account>> GetAllAccountsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = db.Accounts.AsQueryable();
                return PagedResponse<Account>.FromQueryable(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving login infos.", ex);
            }
        }

        public Task<Account?> GetAccountAsync(Guid id)
        {
            // return null if not found
            try
            {
                var loginInfo = db.Accounts.Find(id);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        // Use to login by username and password
        public Task<Account?> GetAccountAsync(string userName)
        {
            try
            {
                var loginInfo = db.Accounts.FirstOrDefault(li => li.userName == userName);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newHashedPassword)
        {
            var account = await db.Accounts.FindAsync(accountId);
            if (account == null)
                return false;
            
            var checkOldPassword = hasherPassword.VerifyPassword(oldPassword, account.passwordHash);
            if (!checkOldPassword)
                return false;
            
            // TODO: Bug ne
            account.passwordHash = hasherPassword.HashPassword(newHashedPassword);
            await db.SaveChangesAsync();
            return true;
        }

        public Task UpdateAccountAsync(Account loginInfo)
        {
            try
            {
                db.Accounts.Update(loginInfo);
                db.SaveChangesAsync();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the login info.", ex);
            }
        }

        public LibraryDbContext GetDbContext()
        {
            return db;
        }
    }
}

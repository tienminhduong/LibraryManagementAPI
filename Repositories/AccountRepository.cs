using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Repositories
{
    public class AccountRepository(LibraryDbContext db) : IAccountRepository
    {
        public Task AddAccountAsync(Account loginInfo)
        {
            try
            {
                db.Accounts.AddAsync(loginInfo);
                db.SaveChangesAsync();
                return Task.CompletedTask;
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
        public Task<Account?> GetAccountAsync(string userName, string password)
        {
            try
            {
                var loginInfo = db.Accounts.FirstOrDefault(li => li.userName == userName && li.passwordHash == password);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
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
    }
}

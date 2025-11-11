using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Repositories
{
    public class LoginInfoRepository(LibraryDbContext db) : ILoginInfoRepository
    {
        public Task AddLoginInfoAsync(LoginInfo loginInfo)
        {
            try
            {
                db.LoginInfos.AddAsync(loginInfo);
                db.SaveChangesAsync();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the login info.", ex);
            }
        }

        public Task DeleteLoginInfoAsync(Guid id)
        {
            try
            {
                var loginInfo = db.LoginInfos.Find(id);
                if (loginInfo != null)
                {
                    db.LoginInfos.Remove(loginInfo);
                    db.SaveChangesAsync();
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the login info.", ex);
            }
        }

        public Task<PagedResponse<LoginInfo>> GetAllLoginInfosAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = db.LoginInfos.AsQueryable();
                return PagedResponse<LoginInfo>.FromQueryable(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving login infos.", ex);
            }
        }

        public Task<LoginInfo?> GetLoginInfoAsync(Guid id)
        {
            // return null if not found
            try
            {
                var loginInfo = db.LoginInfos.Find(id);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        // Use to login by username and password
        public Task<LoginInfo?> GetLoginInfoAsync(string userName, string password)
        {
            try
            {
                var loginInfo = db.LoginInfos.FirstOrDefault(li => li.userName == userName && li.passwordHash == password);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        public Task UpdateLoginInfoAsync(LoginInfo loginInfo)
        {
            try
            {
                db.LoginInfos.Update(loginInfo);
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

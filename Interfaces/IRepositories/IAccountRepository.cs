using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IAccountRepository
    {
        Task<PagedResponse<Account>> GetAllAccountsAsync(int pageNumber, int pageSize);
        Task<Account?> GetAccountAsync(Guid id);
        Task UpdateAccountAsync(Account loginInfo);
        Task DeleteAccountAsync(Guid id);
        Task AddAccountAsync(Account loginInfo, BaseInfo info);
        Task<Account?> GetAccountAsync(string username, string password);
        LibraryDbContext GetDbContext();
    }
}

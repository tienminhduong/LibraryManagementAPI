using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Repositories;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IAccountRepository
    {
        Task<PagedResponse<Account>> GetAllAccountsAsync(int pageNumber, int pageSize);
        Task<Account?> GetAccountAsync(Guid id);
        Task UpdateAccountAsync(Account loginInfo);
        Task DeleteAccountAsync(Guid id);
        Task AddAccountAsync(Account loginInfo, BaseInfo info);
        Task<Account?> GetAccountAsync(string username);
        Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newHashedPassword);
        LibraryDbContext GetDbContext();
        Task<bool> UpdateAccountStatus(Guid accountId, bool status);
        Task<PagedResponse<InFoAccountBorrow>>
            GetInfoBorrowForMemberAccountAsync(string? keyword, int pageNumber = 1, int pageSize = 20);
    }
}

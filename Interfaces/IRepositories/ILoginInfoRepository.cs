using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface ILoginInfoRepository
    {
        Task<PagedResponse<LoginInfo>> GetAllLoginInfosAsync(int pageNumber, int pageSize);
        Task<LoginInfo?> GetLoginInfoAsync(Guid id);
        Task UpdateLoginInfoAsync(LoginInfo loginInfo);
        Task DeleteLoginInfoAsync(Guid id);
        Task AddLoginInfoAsync(LoginInfo loginInfo);
        Task<LoginInfo?> GetLoginInfoAsync(string username, string password);
    }
}

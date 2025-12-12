using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface IInfoRepository
    {
        Task<IEnumerable<BaseInfo>> GetAllAsync();
        Task<BaseInfo?> GetByIdAsync(Guid id);
        Task<BaseInfo?> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(BaseInfo info, bool isInTransaction = false);
        Task UpdateAsync(BaseInfo info);
        Task<bool> IsAccountIdExist(Guid accountId, Role type);
        Task<BaseInfo?> GetInfoByAccountIdAsync(Guid accountId, Role type = Role.Member);
        Task<IEnumerable<Entities.MemberInfo>> SearchMembersAsync(string searchTerm);
    }
}

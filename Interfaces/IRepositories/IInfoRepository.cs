using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public enum InfoType
    {
        Admin,
        Staff,
        Member
    }
    public interface IInfoRepository
    {
        Task<IEnumerable<BaseInfo>> GetAllAsync();
        Task<BaseInfo?> GetByIdAsync(Guid id);
        Task AddAsync(BaseInfo info, bool isInTransaction = false);
        Task UpdateAsync(BaseInfo info);
        Task<bool> IsInfoIdExist(Guid id, InfoType type);
    }
}

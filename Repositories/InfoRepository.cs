using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;

namespace LibraryManagementAPI.Repositories
{
    public class InfoRepository(LibraryDbContext dbContext) : IInfoRepository
    {
        public async Task AddAsync(BaseInfo info, bool isInTransaction = false)
        {
            try
            {
                await dbContext.BaseInfos.AddAsync(info);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the info.", ex);
            }
        }

        public Task<IEnumerable<BaseInfo>> GetAllAsync()
        {
            try
            {
                return Task.FromResult(dbContext.BaseInfos.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving infos.", ex);
            }
        }

        public Task<BaseInfo?> GetByIdAsync(Guid id)
        {
            try
            {
                var info = dbContext.BaseInfos.Find(id);
                return Task.FromResult(info);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the info.", ex);
            }
        }

        public Task UpdateAsync(BaseInfo info)
        {
            try
            {
                dbContext.BaseInfos.Update(info);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the info.", ex);
            }
        }
    }
}

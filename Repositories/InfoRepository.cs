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
                await dbContext.AddAsync(info);
                if (!isInTransaction)
                    await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the info.", ex);
            }
        }

        public Task<IEnumerable<BaseInfo>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseInfo?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(BaseInfo info)
        {
            throw new NotImplementedException();
        }

        //public Task<IEnumerable<BaseInfo>> GetAllAsync()
        //{
        //    try
        //    {
        //        return Task.FromResult(dbContext.AdminInfos.AsEnumerable());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while retrieving infos.", ex);
        //    }
        //}

        //public Task<BaseInfo?> GetByIdAsync(Guid id)
        //{
        //    try
        //    {
        //        var info = dbContext.AdminInfos.FindAsync(id);
        //        return info;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while retrieving the info.", ex);
        //    }
        //}

        //public Task UpdateAsync(BaseInfo info)
        //{
        //    try
        //    {
        //        dbContext.BaseInfos.Update(info);
        //        return Task.CompletedTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while updating the info.", ex);
        //    }
        //}
    }
}

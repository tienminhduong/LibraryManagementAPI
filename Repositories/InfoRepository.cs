using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using System.Reflection;

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

        public async Task<BaseInfo?> GetByIdAsync(Guid id)
        {
            try
            {
                var info = await (dbContext.FindAsync(typeof(BaseInfo), id));
                return (BaseInfo?)info;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while find the info", ex);
            }
        }

        public Task UpdateAsync(BaseInfo info)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsInfoIdExist(Guid id, InfoType type)
        {
            try
            {
                BaseInfo? info = null;
                switch (type)
                {
                    case InfoType.Staff:
                        info = await dbContext.StaffInfos.FindAsync(id);
                        break;
                    case InfoType.Member:
                        info = await dbContext.MemberInfos.FindAsync(id);
                        break;
                    case InfoType.Admin:
                        info = await dbContext.AdminInfos.FindAsync(id);
                        break;
                }
                return info != null;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking the info id existence.", ex);
            }
        }
    }
}

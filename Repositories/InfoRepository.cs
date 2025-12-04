using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> IsInfoIdExist(Guid id, Role type = Role.Member)
        {
            try
            {
                BaseInfo? info = null;
                switch (type)
                {
                    case Role.Staff:
                        info = await dbContext.StaffInfos.FindAsync(id);
                        break;
                    case Role.Member:
                        info = await dbContext.MemberInfos.FindAsync(id);
                        break;
                    case Role.Admin:
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

        public async Task<BaseInfo?> GetInfoByAccountIdAsync(Guid accountId, Role type)
        {
            try
            {
                switch (type)
                {
                    case Role.Staff:
                        return await dbContext.StaffInfos
                                     .Where(info => info.loginId == accountId)
                                     .FirstOrDefaultAsync();
                    case Role.Member:
                        return await dbContext.MemberInfos
                            .Where(info => info.loginId == accountId)
                            .FirstOrDefaultAsync();
                    case Role.Admin:
                        return await dbContext.AdminInfos
                            .Where(info => info.loginId == accountId)
                            .FirstOrDefaultAsync();
                    default:
                        throw new ArgumentException("Invalid InfoType provided.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the info by account id.", ex);
            }
        }
    }
}

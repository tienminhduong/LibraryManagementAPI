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

        public async Task<BaseInfo?> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                // Try to find in each info table
                var staffInfo = await dbContext.StaffInfos
                    .FirstOrDefaultAsync(i => i.loginId == accountId);
                if (staffInfo != null) return staffInfo;

                var memberInfo = await dbContext.MemberInfos
                    .FirstOrDefaultAsync(i => i.loginId == accountId);
                if (memberInfo != null) return memberInfo;

                var adminInfo = await dbContext.AdminInfos
                    .FirstOrDefaultAsync(i => i.loginId == accountId);
                if (adminInfo != null) return adminInfo;

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while finding the info by account ID.", ex);
            }
        }

        public Task UpdateAsync(BaseInfo info)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsAccountIdExist(Guid accountId, Role type = Role.Member)
        {
            try
            {
                BaseInfo? info = null;
                switch (type)
                {
                    case Role.Staff:
                        info = await dbContext.StaffInfos.FirstOrDefaultAsync(i => i.loginId == accountId);
                        break;
                    case Role.Member:
                        info = await dbContext.MemberInfos.FirstOrDefaultAsync(i => i.loginId == accountId);
                        break;
                    case Role.Admin:
                        info = await dbContext.AdminInfos.FirstOrDefaultAsync(i => i.loginId == accountId);
                        break;
                }
                return info != null;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking the account id existence.", ex);
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
                        throw new ArgumentException("Invalid Role type provided.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the info by account id.", ex);
            }
        }

        public async Task<IEnumerable<Entities.MemberInfo>> SearchMembersAsync(string searchTerm)
        {
            try
            {
                var normalizedSearch = searchTerm.ToLower().Trim();
                
                var members = await dbContext.MemberInfos
                    .Where(m => 
                        (m.fullName != null && m.fullName.ToLower().Contains(normalizedSearch)) ||
                        (m.email != null && m.email.ToLower().Contains(normalizedSearch)) ||
                        (m.phoneNumber != null && m.phoneNumber.ToLower().Contains(normalizedSearch)))
                    .Take(20) // Limit results
                    .ToListAsync();
                
                return members;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while searching for members.", ex);
            }
        }

        public async Task<int> GetBorrowCount(Guid memberId)
        {
            return await dbContext.BookTransactions
                .Where(trans => trans.memberId == memberId).CountAsync();
        }

        public async Task<int> GetLateCount(Guid memberId)
        {
            return await dbContext.BookTransactions
                .Where(trans => trans.memberId == memberId)
                .Where(trans => trans.status == StatusTransaction.OVERDUE)
                .CountAsync();
        }
    }
}

using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IUtility;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class AccountRepository(LibraryDbContext db, IHasherPassword hasherPassword) : IAccountRepository
    {
        public async Task AddAccountAsync(Account loginInfo, BaseInfo info)
        {
            try
            {
                // them account truoc
                await db.Accounts.AddAsync(loginInfo); 
                await db.SaveChangesAsync();

                // gan id account vao info
                info.loginId = loginInfo.id;
                // fix lai ngay vi posgres chi chap nhan DateTimeKind.Utc
                info.ConvertTimezone();

                // them info sau
                await db.AddAsync(info);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the login info.", ex);
            }
        }

        public Task DeleteAccountAsync(Guid id)
        {
            try
            {
                var loginInfo = db.Accounts.Find(id);
                if (loginInfo != null)
                {
                    db.Accounts.Remove(loginInfo);
                    db.SaveChangesAsync();
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the login info.", ex);
            }
        }

        public Task<PagedResponse<Account>> GetAllAccountsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = db.Accounts.AsQueryable();
                return PagedResponse<Account>.FromQueryable(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving login infos.", ex);
            }
        }

        public Task<Account?> GetAccountAsync(Guid id)
        {
            // return null if not found
            try
            {
                var loginInfo = db.Accounts.Find(id);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        // Use to login by username and password
        public Task<Account?> GetAccountAsync(string userName)
        {
            try
            {
                var loginInfo = db.Accounts.FirstOrDefault(li => li.userName == userName);
                return Task.FromResult(loginInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the login info.", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newHashedPassword)
        {
            var account = await db.Accounts.FindAsync(accountId);
            if (account == null)
                return false;
            
            var checkOldPassword = hasherPassword.VerifyPassword(oldPassword, account.passwordHash);
            if (!checkOldPassword)
                return false;
            
            // TODO: Bug ne
            account.passwordHash = hasherPassword.HashPassword(newHashedPassword);
            await db.SaveChangesAsync();
            return true;
        }

        public Task UpdateAccountAsync(Account loginInfo)
        {
            try
            {
                db.Accounts.Update(loginInfo);
                db.SaveChangesAsync();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the login info.", ex);
            }
        }

        public LibraryDbContext GetDbContext()
        {
            return db;
        }

        public async Task<bool> UpdateAccountStatus(Guid accountId, bool status)
        {
            var account = await db.Accounts.FindAsync(accountId);
            if (account == null)
                return false;

            account.isActive = status;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResponse<InFoAccountBorrow>> 
            GetInfoBorrowForMemberAccountAsync(int pageNumber = 1, int pageSize = 20)
        {
            //var memberProfiles =
            //                from acc in db.Accounts.AsNoTracking()
            //                where acc.role == Role.Member
            //                join info in db.MemberInfos
            //                    on acc.id equals info.loginId
            //                select new InfoAccount
            //                {
            //                    Id = acc.id,
            //                    IsActive = acc.isActive,
            //                    FullName = info.fullName ?? "",
            //                    Email = info.email ?? ""
            //                };

            //var borrowStatistics =
            //                from br in db.BorrowRequests
            //                group br by br.MemberId
            //                into g
            //                select new BorrowStatistics
            //                {
            //                    MemberId = g.Key,

            //                    TotalBorrowRequests = g.Count(),

            //                    TotalNotReturnOverdues = g.Count(br =>
            //                        br.Status == BorrowRequestStatus.Overdue),

            //                    TotalReturnOverdues = g.Count(br =>
            //                        br.Status == BorrowRequestStatus.OverdueReturned)
            //                };

            //var resultQuery =
            //            from mp in memberProfiles
            //            join bs in borrowStatistics
            //                on mp.Id equals bs.MemberId
            //                into bsg
            //            from bs in bsg.DefaultIfEmpty()   // LEFT JOIN
            //            select new InFoAccountBorrow
            //            {
            //                accountId = mp.Id,
            //                isActive = mp.IsActive,
            //                fullName = mp.FullName,
            //                email = mp.Email,
            //                totalBorrowRequests = bs != null ? bs.TotalBorrowRequests : 0,
            //                totalNotReturnOverdues = bs != null ? bs.TotalNotReturnOverdues : 0,
            //                totalReturnOverdues = bs != null ? bs.TotalReturnOverdues : 0
            //            };
            var resultQuery = db.Accounts
    .AsNoTracking()
    .Where(a => a.role == Role.Member)
    .Select(a => new InFoAccountBorrow
    {
        accountId = a.id,
        isActive = a.isActive,
        fullName = ((MemberInfo)a.info).fullName ?? "",
        email = ((MemberInfo)a.info).email ?? "",

        // Member chưa mượn → COUNT = 0
        totalBorrowRequests = db.BorrowRequests.Count(br => br.MemberId == a.id),

        totalNotReturnOverdues = db.BorrowRequests.Count(br =>
            br.Status == BorrowRequestStatus.Overdue &&
            br.MemberId == a.id
        ),

        totalReturnOverdues = db.BorrowRequests.Count(br =>
            br.Status == BorrowRequestStatus.OverdueReturned &&
            br.MemberId == a.id
        )
    })
    .OrderBy(ib => ib.totalNotReturnOverdues);


            var pagedResult = await PagedResponse<InFoAccountBorrow>.FromQueryable(
                resultQuery.AsNoTracking(),
                pageNumber: pageNumber,
                pageSize: pageSize);

            return pagedResult;

        }
    }

    public class InFoAccountBorrow
    {
        public Guid accountId { get; set; }
        public bool isActive { get; set; }
        public string fullName { get; set; } = "";
        public string email { get; set; } = "";
        public int totalBorrowRequests { get; set; }
        public int totalNotReturnOverdues { get; set; }
        public int totalReturnOverdues { get; set; }
    }

    public class BorrowStatistics
    {
        public Guid? MemberId { get; set; }
        public int TotalBorrowRequests { get; set; }
        public int TotalNotReturnOverdues { get; set; }
        public int TotalReturnOverdues { get; set; }
    }

    public class InfoAccount
    {
        //public AccountActive accountActive;
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}

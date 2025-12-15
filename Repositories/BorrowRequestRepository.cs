using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.User;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class BorrowRequestRepository(LibraryDbContext db) : IBorrowRequestRepository
    {
        public async Task<BorrowRequest?> GetById(Guid id)
        {
            return await db.BorrowRequests.FindAsync(id);
        }

        public async Task<BorrowRequest?> GetByIdWithDetails(Guid id)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                    .ThenInclude(bc => bc!.book)
                .FirstOrDefaultAsync(br => br.Id == id);
        }

        public async Task<IEnumerable<BorrowRequest>> GetAll()
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRequest>> GetByMemberId(Guid memberId)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .Where(br => br.MemberId == memberId)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResponse<BorrowRequest>> GetByMemberIdPaged(Guid memberId, int pageNumber = 1, int pageSize = 20)
        {
            var query = db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .Where(br => br.MemberId == memberId)
                .OrderByDescending(br => br.CreatedAt)
                .AsQueryable();

            return await PagedResponse<BorrowRequest>.FromQueryable(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<BorrowRequest>> GetByStatus(BorrowRequestStatus status)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .Where(br => br.Status == status)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResponse<BorrowRequest>> GetByStatusPaged(BorrowRequestStatus status, int pageNumber = 1, int pageSize = 20)
        {
            var query = db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .Where(br => br.Status == status)
                .OrderByDescending(br => br.CreatedAt)
                .AsQueryable();

            return await PagedResponse<BorrowRequest>.FromQueryable(query, pageNumber, pageSize);
        }

        public async Task Add(BorrowRequest borrowRequest)
        {
            await db.BorrowRequests.AddAsync(borrowRequest);
        }

        public Task Update(BorrowRequest borrowRequest)
        {
            db.BorrowRequests.Update(borrowRequest);
            return Task.CompletedTask;
        }

        public async Task Delete(Guid id)
        {
            var borrowRequest = await db.BorrowRequests.FindAsync(id);
            if (borrowRequest != null)
            {
                db.BorrowRequests.Remove(borrowRequest);
            }
        }
        
        public async Task<BorrowRequest?> GetByBookCopyIdAsync(Guid bookCopyId)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .FirstOrDefaultAsync(br => 
                    br.BookCopyId == bookCopyId &&
                    (br.Status == BorrowRequestStatus.Borrowed || 
                     br.Status == BorrowRequestStatus.Overdue));
        }
        
        public async Task<BorrowRequest?> GetByQrCodeAsync(string qrCode)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .FirstOrDefaultAsync(br => br.QrCode == qrCode);
        }

        public async Task<PagedResponse<BorrowRequest>> GetByMemberIdAndStatusPaged(Guid memberId, BorrowRequestStatus? status, int pageNumber = 1, int pageSize = 20)
        {
            var query = db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .Where(br => br.MemberId == memberId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(br => br.Status == status.Value);
            }

            query = query.OrderByDescending(br => br.CreatedAt);

            return await PagedResponse<BorrowRequest>.FromQueryable(query, pageNumber, pageSize);
        }

        public async Task<PagedResponse<BorrowRequest>> GetAllByStatusPaged(BorrowRequestStatus? status, int pageNumber = 1, int pageSize = 20)
        {
            var query = db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Book)
                .Include(br => br.BookCopy)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(br => br.Status == status.Value);
            }

            query = query.OrderByDescending(br => br.CreatedAt);

            return await PagedResponse<BorrowRequest>.FromQueryable(query, pageNumber, pageSize);
        }

        public async Task<PagedResponse<LateReturnedUserDto>> GetBorrowCountForMemberAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = db.BorrowRequests
                .AsNoTracking()
                .Where(br => br.Status == BorrowRequestStatus.Overdue ||
                            br.Status == BorrowRequestStatus.OverdueReturned)
                .GroupBy(br => br.MemberId)
                .Select(g => new LateReturnedUser
                {
                    UserId = g.Key,
                    LateReturnsCount = g.Count(br => br.Status == BorrowRequestStatus.OverdueReturned),
                    LateNotReturnedCount = g.Count(br => br.Status == BorrowRequestStatus.Overdue),
                    BorrowCount = g.Count()
                });

            var nextQuery = db.MemberInfos
                .AsNoTracking()
                .Join(query,
                    mi => mi.id,
                    lru => lru.UserId,
                    (mi, lru) => new LateReturnedUserMember
                    {
                        UserId = lru.UserId,
                        LateReturnsCount = lru.LateReturnsCount,
                        LateNotReturnedCount = lru.LateNotReturnedCount,
                        BorrowCount = lru.BorrowCount,
                        FullName = mi.fullName ?? "",
                        Email = mi.email ?? "",
                        LoginId = mi.loginId
                    });

            var finalQuery = db.Accounts
                .Join(nextQuery,
                    acc => acc.id,
                    lrum => lrum.LoginId,
                    (acc, lrum) => new LateReturnedUserDto
                    {
                        UserId = lrum.UserId,
                        LateReturnsCount = lrum.LateReturnsCount,
                        LateNotReturnedCount = lrum.LateNotReturnedCount,
                        BorrowCount = lrum.BorrowCount,
                        FullName = lrum.FullName,
                        Email = lrum.Email,
                        IsActive = acc.isActive
                    })
                .OrderBy(u => u.LateNotReturnedCount);

            var res = await PagedResponse<LateReturnedUserDto>.FromQueryable(finalQuery, pageNumber, pageSize);
            return res;
        }

        
    }
}

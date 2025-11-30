using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
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
                .Include(br => br.Items)
                    .ThenInclude(i => i.Book)
                .Include(br => br.Items)
                    .ThenInclude(i => i.BookCopy)
                .FirstOrDefaultAsync(br => br.Id == id);
        }

        public async Task<IEnumerable<BorrowRequest>> GetAll()
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Items)
                    .ThenInclude(i => i.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRequest>> GetByMemberId(Guid memberId)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Items)
                    .ThenInclude(i => i.Book)
                .Where(br => br.MemberId == memberId)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRequest>> GetByStatus(BorrowRequestStatus status)
        {
            return await db.BorrowRequests
                .Include(br => br.Member)
                .Include(br => br.Staff)
                .Include(br => br.Items)
                    .ThenInclude(i => i.Book)
                .Where(br => br.Status == status)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
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
    }
}

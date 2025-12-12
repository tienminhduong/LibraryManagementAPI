using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
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
    }
}

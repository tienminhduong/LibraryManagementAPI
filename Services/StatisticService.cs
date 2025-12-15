using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Statistic;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public class StatisticService(LibraryDbContext dbContext) : IStatisticService
{
    public async Task<BorrowCountStatDto?> GetBorrowCountStat(DateTime startDate, DateTime endDate)
    {
        startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);
        
        var query = await dbContext.BorrowRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Where(br => br.ConfirmedAt != null && br.ConfirmedAt >= startDate && br.ConfirmedAt <= endDate)
            .GroupBy(br => br.ConfirmedAt)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
        
        var totalDays = (endDate - startDate).Days + 1;
        var borrowCounts = Enumerable.Repeat(0, totalDays).ToList();
        var total = 0;
        
        foreach (var record in query)
        {
            var date = record.Date!.Value.Date;
            var index = (date - startDate).Days;
            
            if (index < 0 || index >= totalDays) continue;
            
            borrowCounts[index] = record.Count;
            total += record.Count;
        }
        
        return new BorrowCountStatDto
        {
            FromDate = startDate,
            ToDate = endDate,
            DailyCounts = borrowCounts,
            Total =  total
        };
    }

    public async Task<IEnumerable<MemberBorrowCountStatDto>> GetTopMembersByBorrowCount(DateTime startDate, DateTime endDate, int topN = 5)
    {
        startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

        var query = await dbContext.BorrowRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Where(br => br.ConfirmedAt != null && br.ConfirmedAt >= startDate && br.ConfirmedAt <= endDate)
            .GroupBy(br => new { br.MemberId, br.Member!.fullName })
            .Select(g => new MemberBorrowCountStatDto
            {
                MemberName = g.Key.fullName ?? "Unknown",
                BorrowCount = g.Count()
            })
            .OrderByDescending(dto => dto.BorrowCount)
            .Take(topN)
            .ToListAsync();

        return query;
    }

    public async Task<TotalCountsDto> GetTotalCounts()
    {
        var totalMembers = await dbContext.MemberInfos.CountAsync();
        var totalBooks = await dbContext.Books.CountAsync();
        var totalBorrowRequests = await dbContext.BorrowRequests.CountAsync();

        return new TotalCountsDto
        {
            TotalMembers = totalMembers,
            TotalBooks = totalBooks,
            TotalBorrowRequests = totalBorrowRequests
        };
    }
}
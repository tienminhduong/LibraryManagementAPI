using LibraryManagementAPI.Context;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Statistic;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public class StatisticService(LibraryDbContext dbContext) : IStatisticService
{
    public async Task<BorrowCountStatDto?> GetBorrowCountStat(DateTime startDate, DateTime endDate)
    {
        //check start date and end date are in utc format
        startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);
        
        var query = await dbContext.BorrowRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Where(br => br.ConfirmedAt != null && br.ConfirmedAt >= startDate && br.ConfirmedAt <= endDate)
            .OrderBy(br => br.ConfirmedAt)
            .ToListAsync();
        
        var totalDays = (endDate - startDate).Days + 1;
        var borrowCounts = Enumerable.Repeat(0, totalDays).ToList();
        var total = 0;
        
        foreach (var dayIndex in query.Select(request => (request.ConfirmedAt!.Value - startDate).Days))
        {
            borrowCounts[dayIndex]++;
            total++;
        }
        
        return new BorrowCountStatDto
        {
            FromDate = startDate,
            ToDate = endDate,
            DailyCounts = borrowCounts,
            Total =  total
        };
    }
}
using LibraryManagementAPI.Models.Statistic;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IStatisticService
{
    Task<BorrowCountStatDto?> GetBorrowCountStat(DateTime startDate, DateTime endDate);
    Task<IEnumerable<MemberBorrowCountStatDto>> GetTopMembersByBorrowCount(DateTime startDate, DateTime endDate, int topN = 5);
    Task<TotalCountsDto> GetTotalCounts();
}
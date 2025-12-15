using LibraryManagementAPI.Models.Statistic;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IStatisticService
{
    Task<BorrowCountStatDto?> GetBorrowCountStat(DateTime startDate, DateTime endDate);
}
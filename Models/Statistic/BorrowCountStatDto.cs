namespace LibraryManagementAPI.Models.Statistic;

public class BorrowCountStatDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    
    public int Total { get; set; }
    public List<int> DailyCounts { get; set; } = [];
}
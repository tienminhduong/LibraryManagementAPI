namespace LibraryManagementAPI.Models.Statistic;

public class MemberBorrowCountStatDto
{
    public required string MemberName { get; set; }
    public required int BorrowCount { get; set; }
}
using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Models.Utility
{
    public static class Utility
    {
        static public string GetClaim(HttpContext httpContext, string claimType)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value ?? string.Empty;
        }

        public static (DateTime From, DateTime To) ResolveRangeTime(DateTime? from, DateTime? to)
        {
            var now = DateTime.UtcNow;

            var start = from?.ToUniversalTime()
                ?? new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var end = to?.ToUniversalTime()
                ?? start.AddMonths(1).AddTicks(-1);

            return (start, end);
        }

        public static readonly BorrowRequestStatus[] validStatuses =
    new[] { BorrowRequestStatus.Borrowed,
            BorrowRequestStatus.Returned,
            BorrowRequestStatus.Overdue,
            BorrowRequestStatus.OverdueReturned};
    }
}

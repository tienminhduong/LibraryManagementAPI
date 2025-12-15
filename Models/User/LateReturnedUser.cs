using Org.BouncyCastle.Bcpg.OpenPgp;

namespace LibraryManagementAPI.Models.User
{
    public class LateReturnedUser
    {
        public Guid UserId { get; set; }
        public int LateReturnsCount { get; set; }
        public int LateNotReturnedCount { get; set; }
        public int BorrowCount { get; set; }
    }

    public class LateReturnedUserDto
    {
        public Guid UserId { get; set; }
        public int LateReturnsCount { get; set; }
        public int LateNotReturnedCount { get; set; }
        public int BorrowCount { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class LateReturnedUserMember
    {
        public Guid UserId { get; set; }
        public int LateReturnsCount { get; set; }
        public int LateNotReturnedCount { get; set; }
        public int BorrowCount { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Guid LoginId { get; set; }
    }

    public class ProfileActive()
    {
        public LateReturnedUser lateReturnedUser { get; set; }
        public bool isActive { get; set; }
    }
}

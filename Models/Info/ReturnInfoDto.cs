namespace LibraryManagementAPI.Models.Info
{
    public class ReturnInfoDto
    {
    }

    public class ReturnAdminInfoDto
    {
        public string fullName { get; set; } = null!;
        public string email { get; set; } = null!;
        public string phoneNumber { get; set; } = null!;
    }

    public class ReturnStaffInfoDto
    {
        public string fullName { get; set; } = null!;
        public string email { get; set; } = null!;
        public string phoneNumber { get; set; } = null!;
        public DateTime hireDate { get; set; }
    }

    public class ReturnMemberInfoDto
    {
        public string fullName { get; set; } = null!;
        public string email { get; set; } = null!;
        public string? phoneNumber { get; set; } = null!;
        public string? address { get; set; } = null!;
        public string? imageUrl { get; set; } = null!;
        public DateTime joinDate { get; set; }

    }

}

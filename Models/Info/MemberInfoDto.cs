namespace LibraryManagementAPI.Models.Info
{
    public class MemberInfoDto: BaseInfoDto
    {
        public string? address { get; set; }
        public string? imageUrl { get; set; }
        public DateTime joinDate { get; set; } = DateTime.UtcNow;

    }
}

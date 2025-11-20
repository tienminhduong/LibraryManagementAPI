namespace LibraryManagementAPI.Models.Info
{
    public class StaffInfoDto: BaseInfoDto
    {
        public DateTime hireDate { get; set; } = DateTime.UtcNow;
    }
}

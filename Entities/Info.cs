using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public abstract class BaseInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public Guid loginId { get; set; }
        [ForeignKey("loginId")]
        public virtual Account? account { get; set; }
        public virtual void ConvertTimezone() { }
    }
    public class StaffInfo: BaseInfo
    {
        public DateTime hireDate { get; set; } = DateTime.UtcNow;
        public override void ConvertTimezone()
        {
            hireDate = DateTime.SpecifyKind(hireDate, DateTimeKind.Utc);
        }
    }

    public class MemberInfo: BaseInfo
    {
        public string? address { get; set; }
        public string? imageUrl { get; set; }
        public DateTime joinDate { get; set; } = DateTime.UtcNow;
        public override void ConvertTimezone()
        {
            joinDate = DateTime.SpecifyKind(joinDate, DateTimeKind.Utc);
        }
    }
    public class AdminInfo: BaseInfo
    {
        
    }
}

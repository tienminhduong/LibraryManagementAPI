using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class StaffInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }

        public Guid? loginId { get; set; }
        [ForeignKey("loginId")]
        public virtual Account? LoginInfoId { get; set; }
        public DateTime hireDate { get; set; } = DateTime.Today;
    }

    public class MemberInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }

        public Guid? loginId { get; set; }
        [ForeignKey("loginId")]
        public virtual Account? LoginInfoId { get; set; }
        public string? address { get; set; }
        public string? imageUrl { get; set; }
        public DateTime joinDate { get; set; } = DateTime.Today;
    }
    public class AdminInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }

        public Guid? loginId { get; set; }
        [ForeignKey("loginId")]
        public virtual Account? LoginInfoId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public enum Role
    {
        Admin,
        Staff,
        Member,
        Guest
    }
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public required string userName { get; set; }
        public required string passwordHash { get; set; }
        public Role role { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime lastLogin { get; set; } = DateTime.UtcNow;
        public bool isActive { get; set; } = true;
        public BaseInfo? info { get; set; }
    }

}

using LibraryManagementAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Models.Account
{
    public class AccountDto
    {
        public Guid id { get; set; }
        public required string userName { get; set; }
        public required string passwordHash { get; set; }
        public Role role { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime lastLogin { get; set; }
        public bool isActive { get; set; } = true;
        public AdminInfo? AdminInfo { get; set; }
    }
}

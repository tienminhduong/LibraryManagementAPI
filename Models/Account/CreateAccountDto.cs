using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Info;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Models.Account
{
    public class CreateAccountDto
    {
        public required string userName { get; set; }
        public required string password { get; set; }
        public Role role { get; set; }
        public required AdminInfoDto adminInfo { get; set; }
    }
}

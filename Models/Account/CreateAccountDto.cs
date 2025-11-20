using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Info;
using System.Text.Json.Serialization;

namespace LibraryManagementAPI.Models.Account
{
    public class CreateAccountDto
    {
        public required string userName { get; set; }
        public required string password { get; set; }
        public required Role role { get; set; }
        public required BaseInfoDto info { get; set; }
    }

    //public class CreateAdminAccountDto : CreateAccountDto
    //{
    //    public required AdminInfo info { get; set; }
    //}

    //public class CreateStaffAccountDto : CreateAccountDto
    //{
    //    public required StaffInfo info { get; set; }
    //}

    //public class CreateMemberAccountDto : CreateAccountDto
    //{
    //    public required MemberInfo info { get; set; }
    //}
}

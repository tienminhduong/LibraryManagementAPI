using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagementAPI.Models.Info
{
    [JsonDerivedType(typeof(AdminInfoDto), "admin")]
    [JsonDerivedType(typeof(StaffInfoDto), "staff")]
    [JsonDerivedType(typeof(MemberInfoDto), "member")]
    public class BaseInfoDto
    {
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
    }
}

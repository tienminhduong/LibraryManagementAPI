using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Info;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IProfileService
    {
        public Task<Response<ReturnAdminInfoDto>> GetAdminProfileByAccountIdAsync(Guid accountId);
        public Task<Response<ReturnStaffInfoDto>> GetStaffProfileByAccountIdAsync(Guid accountId);
        public Task<Response<ReturnMemberInfoDto>> GetMemberProfileByAccountIdAsync(Guid accountId);
    }
}

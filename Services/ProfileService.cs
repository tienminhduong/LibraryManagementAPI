using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Info;

namespace LibraryManagementAPI.Services
{
    public class ProfileService(IInfoRepository infoRepository) : IProfileService
    {
        public async Task<Response<ReturnAdminInfoDto>> GetAdminProfileByAccountIdAsync(Guid accountId)
        {
            try
            {
                var res = await infoRepository.GetByAccountIdAsync(accountId, InfoType.Admin);
                if (res == null)
                {
                    return Response<ReturnAdminInfoDto>.Failure("Profile not found");
                }

                var adminInfo = (AdminInfo)res;
                var returnDto = new ReturnAdminInfoDto
                {
                    fullName = adminInfo.fullName,
                    email = adminInfo.email,
                    phoneNumber = adminInfo.phoneNumber
                };
                return Response<ReturnAdminInfoDto>.Success(returnDto);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while retrieving the profile.", ex);
            }
        }

        public async Task<Response<ReturnMemberInfoDto>> GetMemberProfileByAccountIdAsync(Guid accountId)
        {
            try
            {
                var res = await infoRepository.GetByAccountIdAsync(accountId, InfoType.Member);
                if (res == null)
                {
                    return Response<ReturnMemberInfoDto>.Failure("Profile not found");
                }
                var memberInfo = (MemberInfo)res;
                var returnDto = new ReturnMemberInfoDto
                {
                    fullName = memberInfo.fullName,
                    email = memberInfo.email,
                    phoneNumber = memberInfo.phoneNumber,
                    joinDate = memberInfo.joinDate,
                    address = memberInfo.address,
                    imageUrl = memberInfo.imageUrl
                };
                return Response<ReturnMemberInfoDto>.Success(returnDto);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the profile.", ex);
            }
        }

        public async Task<Response<ReturnStaffInfoDto>> GetStaffProfileByAccountIdAsync(Guid accountId)
        {   
            try
            {
                var res = await infoRepository.GetByAccountIdAsync(accountId, InfoType.Staff);
                if (res == null)
                {
                    return Response<ReturnStaffInfoDto>.Failure("Profile not found");
                }
                var staffInfo = (StaffInfo)res;
                var returnDto = new ReturnStaffInfoDto
                {
                    fullName = staffInfo.fullName,
                    email = staffInfo.email,
                    phoneNumber = staffInfo.phoneNumber,
                    hireDate = staffInfo.hireDate
                };
                return Response<ReturnStaffInfoDto>.Success(returnDto);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the profile.", ex);
            }
        }
    }
}

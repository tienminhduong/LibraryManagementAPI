using LibraryManagementAPI.Controllers;
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
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Admin);
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
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Member);
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

        public async Task UpdateAdminProfileAsync(Guid accountId, UpdateProfileRequest updateRequest)
        {
            try
            {
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Admin);
                if (res == null)
                {
                    throw new Exception("Profile not found");
                }
                var adminInfo = (AdminInfo)res;

                if (updateRequest.fullName != null)
                {
                    adminInfo.fullName = updateRequest.fullName;
                }
                if (updateRequest.email != null)
                {
                    adminInfo.email = updateRequest.email;
                }
                if (updateRequest.phoneNumber != null)
                {
                    adminInfo.phoneNumber = updateRequest.phoneNumber;
                }

                await infoRepository.UpdateAsync(adminInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the profile.", ex);
            }
        }

        public async Task UpdateMemberProfileAsync(Guid accountId, UpdateProfileRequest updateRequest)
        {
            try
            {
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Member);
                if (res == null)
                {
                    throw new Exception("Profile not found");
                }
                var memberInfo = (MemberInfo)res;

                if (updateRequest.fullName != null)
                {
                    memberInfo.fullName = updateRequest.fullName;
                }
                if (updateRequest.email != null)
                {
                    memberInfo.email = updateRequest.email;
                }
                if (updateRequest.phoneNumber != null)
                {
                    memberInfo.phoneNumber = updateRequest.phoneNumber;
                }
                if (updateRequest.address != null)
                {
                    memberInfo.address = updateRequest.address;
                }
                if (updateRequest.imageUrl != null)
                {
                    memberInfo.imageUrl = updateRequest.imageUrl;
                }

                await infoRepository.UpdateAsync(memberInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the profile.", ex);
            }
        }

        public async Task UpdateStaffProfileAsync(Guid accountId, UpdateProfileRequest updateRequest)
        {
            try
            {
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Staff);
                if (res == null)
                {
                    throw new Exception("Profile not found");
                }
                var staffInfo = (StaffInfo)res;

                if (updateRequest.fullName != null)
                {
                    staffInfo.fullName = updateRequest.fullName;
                }
                if (updateRequest.email != null)
                {
                    staffInfo.email = updateRequest.email;
                }
                if (updateRequest.phoneNumber != null)
                {
                    staffInfo.phoneNumber = updateRequest.phoneNumber;
                }

                await infoRepository.UpdateAsync(staffInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the profile.", ex);
            }
        }

        public async Task<Response<ReturnStaffInfoDto>> GetStaffProfileByAccountIdAsync(Guid accountId)
        {   
            try
            {
                var res = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Staff);
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

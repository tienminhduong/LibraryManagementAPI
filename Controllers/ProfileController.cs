using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController(IProfileService profileService, IInfoRepository infoRepo) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            // Get Account ID from JWT
            var accountId = User.GetUserId();
            var role = User.GetUserRole();
            
            if (accountId == Guid.Empty)
            {
                return Unauthorized(new { message = "Invalid or missing account ID." });
            }
            
            // check role and get it from service
            var typeRole = TypeRoleMap(role);
            switch (typeRole)
            {
                case Role.Admin:
                    {
                        var res = await profileService.GetAdminProfileByAccountIdAsync(accountId);
                        if(res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                    }
                case Role.Member:
                    {
                        var res = await profileService.GetMemberProfileByAccountIdAsync(accountId);
                        if (res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                    }
                case Role.Staff:
                    {
                        var res = await profileService.GetStaffProfileByAccountIdAsync(accountId);
                        if (res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                    }
                case Role.Guest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }    
            return Unauthorized(new { message = "Unauthorized role." });

        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest updateRequest)
        {
            // Get Account ID from JWT
            var accountId = User.GetUserId();
            var role = User.GetUserRole();
            
            if (accountId == Guid.Empty)
            {
                return Unauthorized(new { message = "Invalid or missing account ID." });
            }
            
            // check role and get it from service
            var typeRole = TypeRoleMap(role);
            switch (typeRole)
            {
                case Role.Admin:
                    {
                        await profileService.UpdateAdminProfileAsync(accountId, updateRequest);
                        return Ok(new { message = "Profile updated successfully." });
                    }
                case Role.Member:
                    {
                        await profileService.UpdateMemberProfileAsync(accountId, updateRequest);
                        return Ok(new { message = "Profile updated successfully." });
                    }
                case Role.Staff:
                    {
                        await profileService.UpdateStaffProfileAsync(accountId, updateRequest);
                        return Ok(new { message = "Profile updated successfully." });
                    }
                case Role.Guest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }    
            return Unauthorized(new { message = "Unauthorized role." });
        }

        /// <summary>
        /// Admin gets user profile by account ID
        /// </summary>
        [HttpGet("user/{accountId}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetUserProfileById(Guid accountId)
        {
            var info = await infoRepo.GetByAccountIdAsync(accountId);
            if (info == null)
                return NotFound(new { message = "User not found." });

            switch (info)
            {
                case AdminInfo:
                    var adminRes = await profileService.GetAdminProfileByAccountIdAsync(accountId);
                    return adminRes.isSuccess ? Ok(adminRes.data) : NotFound(new { message = adminRes.errorMessage });
                case StaffInfo:
                    var staffRes = await profileService.GetStaffProfileByAccountIdAsync(accountId);
                    return staffRes.isSuccess ? Ok(staffRes.data) : NotFound(new { message = staffRes.errorMessage });
                case MemberInfo:
                    var memberRes = await profileService.GetMemberProfileByAccountIdAsync(accountId);
                    return memberRes.isSuccess ? Ok(memberRes.data) : NotFound(new { message = memberRes.errorMessage });
                default:
                    return BadRequest(new { message = "Invalid user type." });
            }
        }

        private Role TypeRoleMap(string role)
        {
            return role switch
            {
                "Admin" => Role.Admin,
                "Staff" => Role.Staff,
                "Member" => Role.Member,
                _ => Role.Guest
            };
        }
    }

    public class UpdateProfileRequest
    {
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? address { get; set; }
        public string? imageUrl { get; set; }
    }
}

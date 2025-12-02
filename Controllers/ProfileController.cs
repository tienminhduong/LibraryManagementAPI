using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var accountId = Utility.GetClaim(HttpContext, CustomClaims.AccountId);
            var role = Utility.GetClaim(HttpContext, CustomClaims.Role);
            if (string.IsNullOrEmpty(accountId) || !Guid.TryParse(accountId, out var parsedAccountId))
            {
                return (Unauthorized(new { message = "Invalid or missing account ID." }));
            }
            // check role and get it from service
            var typeRole = TypeRoleMap(role);
            switch (typeRole)
            {
                case Role.Admin:
                    {
                        var res = await profileService.GetAdminProfileByAccountIdAsync(parsedAccountId);
                        if(res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                        break;
                    }
                case Role.Member:
                    {
                        var res = await profileService.GetMemberProfileByAccountIdAsync(parsedAccountId);
                        if (res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                        break;
                    }
                case Role.Staff:
                    {
                        var res = await profileService.GetStaffProfileByAccountIdAsync(parsedAccountId);
                        if (res.isSuccess)
                        {
                            return Ok(res.data);
                        }
                        else
                        {
                            return NotFound(new { message = res.errorMessage });
                        }
                        break;
                    }
            }    
            return Unauthorized(new { message = "Unauthorized role." });

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
}

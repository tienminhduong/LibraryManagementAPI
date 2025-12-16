using CloudinaryDotNet.Actions;
using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Authorize(Policy = Policies.AdminOnly)]
    [Route("api/member")]
    public class MemberController(
        IAccountService accountService) : ControllerBase
    {
        [HttpGet("member-overdue")]
        public async Task<IActionResult> GetAllMemberOverdue(string? keyword=null, int pageNumber = 1, int pageSize = 20)
        {
            var res = await accountService.GetInfoAccountBorrow(keyword, pageNumber, pageSize);
            if(res == null)
            {
                return NotFound("No overdue members found.");
            }
            return Ok(res);
        }

        [HttpPost("ban")]
        public async Task<IActionResult> BanMember(Guid accountMemberId)
        {
            var res = await accountService.BanAccount(accountMemberId);
            return res ? NoContent() : BadRequest("Failed to ban the account.");
        }
    }
}

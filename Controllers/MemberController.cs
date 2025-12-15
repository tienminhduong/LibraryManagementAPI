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
    public class MemberController(IBorrowRequestService service) : ControllerBase
    {
        [HttpGet("member-overdue")]
        public async Task<IActionResult> GetAllMemberOverdue(int pageNumber = 1, int pageSize = 20)
        {
            var res = await service.GetInfoOverdueForMemberAsync(pageNumber, pageSize);
            if (res.isSuccess)
                return Ok(res);
            else
                return BadRequest(res);
        }
    }
}

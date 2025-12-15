using LibraryManagementAPI.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/staffs")]
public class StaffController(IStaffService staffService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStaffs(Guid id)
    {
        var staffInfoDto = await staffService.GetStaffInfoAsync(id);
        return Ok(staffInfoDto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchStaffs([FromQuery] string? nameContains = null,
        [FromQuery] string? emailContains = null,
        [FromQuery] string? phoneContains = null,
        [FromQuery] DateTime? hiredFrom = null,
        [FromQuery] DateTime? hiredTo = null,
        [FromQuery] DateTime? hiredDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var pagedStaffs = await staffService.SearchStaffsAsync(nameContains, emailContains, phoneContains,
            hiredFrom, hiredTo, hiredDate, pageNumber, pageSize);
        return Ok(pagedStaffs);
    }
}
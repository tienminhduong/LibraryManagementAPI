using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticController(IStatisticService statisticService) : ControllerBase
{
    [HttpGet("by-borrow-count")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> GetBorrowCountStat(DateTime fromDate, DateTime toDate)
    {
        var result = await statisticService.GetBorrowCountStat(fromDate, toDate);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    
    [HttpGet("by-member-count")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> GetTopMembersByBorrowCount(DateTime fromDate, DateTime toDate, int topN = 5)
    {
        var result = await statisticService.GetTopMembersByBorrowCount(fromDate, toDate, topN);
        return Ok(result);
    }
    [HttpGet("total")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> GetTotalCounts()
    {
        var result = await statisticService.GetTotalCounts();
        return Ok(result);
    }
}
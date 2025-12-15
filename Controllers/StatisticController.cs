using LibraryManagementAPI.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticController(IStatisticService statisticService) : ControllerBase
{
    [HttpGet("by-borrow-count")]
    public async Task<ActionResult> GetBorrowCountStat(DateTime fromDate, DateTime toDate)
    {
        var result = await statisticService.GetBorrowCountStat(fromDate, toDate);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
}
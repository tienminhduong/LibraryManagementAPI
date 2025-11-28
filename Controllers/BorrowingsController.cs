using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    public class BorrowingsController(IBorrowBookService service) : ControllerBase
    {
        [HttpPost("api/borrowings/borrow")]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowBookDto borrow)
        {
            try
            {
                var result = await service.BorrowBookAsync(borrow);
                if (result)
                    return Ok(new { message = "Book borrowed successfully." });
                else
                    return BadRequest(new { message = "Failed to borrow book." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

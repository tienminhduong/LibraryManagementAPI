using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.BorrowRequest;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/borrow-requests")]
    public class BorrowRequestController(IBorrowRequestService service) : ControllerBase
    {
        /// <summary>
        /// User creates a borrow request with multiple books in cart
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] CreateBorrowRequestDto dto)
        {
            try
            {
                var result = await service.CreateBorrowRequestAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get borrow request by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBorrowRequest(Guid id)
        {
            try
            {
                var result = await service.GetBorrowRequestByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Borrow request not found." });
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin scans QR code to get borrow request details
        /// </summary>
        [HttpGet("qr/{qrCode}")]
        public async Task<IActionResult> GetBorrowRequestByQrCode(string qrCode)
        {
            try
            {
                var result = await service.GetBorrowRequestByQrCodeAsync(qrCode);
                if (result == null)
                    return NotFound(new { message = "Borrow request not found." });
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin confirms borrow request after scanning books
        /// </summary>
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmBorrowRequest([FromBody] ConfirmBorrowRequestDto dto)
        {
            try
            {
                var result = await service.ConfirmBorrowRequestAsync(dto);
                if (result)
                    return Ok(new { message = "Borrow request confirmed successfully." });
                else
                    return BadRequest(new { message = "Failed to confirm borrow request." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin rejects borrow request
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectBorrowRequest(Guid id, [FromBody] RejectRequestDto dto)
        {
            try
            {
                var result = await service.RejectBorrowRequestAsync(id, dto.StaffId, dto.Reason);
                if (result)
                    return Ok(new { message = "Borrow request rejected." });
                else
                    return BadRequest(new { message = "Failed to reject borrow request." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Member cancels their pending borrow request
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBorrowRequest(Guid id, [FromBody] CancelRequestDto dto)
        {
            try
            {
                var result = await service.CancelBorrowRequestAsync(id, dto.MemberId);
                if (result)
                    return Ok(new { message = "Borrow request cancelled." });
                else
                    return BadRequest(new { message = "Failed to cancel borrow request." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all pending borrow requests (for admin)
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var result = await service.GetPendingRequestsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get borrow requests for a specific member
        /// </summary>
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMemberRequests(Guid memberId)
        {
            try
            {
                var result = await service.GetMemberRequestsAsync(memberId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin scans a book to return it (individual book return)
        /// </summary>
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookDto dto)
        {
            try
            {
                var result = await service.ReturnBookAsync(dto);
                if (result)
                    return Ok(new { message = "Book returned successfully." });
                else
                    return BadRequest(new { message = "Failed to return book." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // Helper DTOs for controller actions
    public class RejectRequestDto
    {
        public Guid StaffId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CancelRequestDto
    {
        public Guid MemberId { get; set; }
    }
}

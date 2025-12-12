using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.BorrowRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/borrow-requests")]
    [Authorize] // All endpoints require authentication
    public class BorrowRequestController(IBorrowRequestService service) : ControllerBase
    {
        /// <summary>
        /// Member creates a borrow request with multiple books
        /// 
        /// NEW RECOMMENDED FLOW (with Cart):
        /// 1. Member adds books to cart: POST /api/cart/add
        /// 2. Member reviews cart: GET /api/cart
        /// 3. Member creates request from cart: POST /api/cart/checkout
        /// 
        /// ALTERNATIVE FLOW (direct, without Cart):
        /// - Member can still directly create borrow request with book IDs (this endpoint)
        /// - Useful for quick single-book borrowing
        /// - Books will be automatically removed from cart if they exist there
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Policies.MemberOnly)]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] CreateBorrowRequestDto dto)
        {
            try
            {
                // Get the authenticated member's account ID from JWT
                var accountId = User.GetUserId();
                
                // Create borrow request directly with book IDs
                // Books will be automatically removed from cart if present
                var result = await service.CreateBorrowRequestAsync(dto, accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin/Staff creates a borrow request for a member with specific book copies
        /// 
        /// ADMIN FLOW:
        /// 1. Admin searches for member: GET /api/borrow-requests/search-members?searchTerm={term}
        /// 2. Admin scans book copy QR codes to get BookCopyIds
        /// 3. Admin creates request with member ID and book copy IDs (this endpoint)
        /// 4. Request is created and immediately confirmed
        /// 5. Book copies are marked as borrowed
        /// 6. Book transactions are created
        /// </summary>
        [HttpPost("admin-create")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> AdminCreateBorrowRequest([FromBody] AdminCreateBorrowRequestDto dto)
        {
            try
            {
                // Get the authenticated staff/admin's account ID from JWT
                var accountId = User.GetUserId();
                
                // Create and confirm borrow request with specific book copies
                var result = await service.AdminCreateBorrowRequestAsync(dto, accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin/Staff searches for members by name, email, or phone number
        /// 
        /// SEARCH FLOW:
        /// - Used in admin borrow request creation flow
        /// - Returns up to 20 matching members
        /// - Search is case-insensitive and matches partial text
        /// </summary>
        [HttpGet("search-members")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> SearchMembers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { message = "Search term is required." });

                var result = await service.SearchMembersAsync(searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get borrow request by ID
        /// 
        /// MEMBER FLOW:
        /// - After creating request (from cart or direct), member can view details
        /// - Member can only view their own requests
        /// 
        /// STAFF/ADMIN FLOW:
        /// - Can view any borrow request
        /// - Used to check request details before confirmation
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBorrowRequest(Guid id)
        {
            try
            {
                var accountId = User.GetUserId();
                var result = await service.GetBorrowRequestByIdAsync(id, accountId, User.GetUserRole());
                
                if (result == null)
                    return NotFound(new { message = "Borrow request not found or access denied." });
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin/Staff scans QR code to get borrow request details
        /// 
        /// LIBRARIAN FLOW:
        /// 1. Member shows QR code (from their phone/email)
        /// 2. Librarian scans QR code with this endpoint
        /// 3. System displays request details and list of books
        /// 4. Librarian proceeds to scan book copies for confirmation
        /// </summary>
        [HttpGet("qr/{qrCode}")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
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
        /// Admin/Staff confirms borrow request after scanning physical books
        /// 
        /// CONFIRMATION FLOW:
        /// 1. Librarian scans member's QR code (GET /api/borrow-requests/qr/{qrCode})
        /// 2. System shows list of requested books
        /// 3. For each book, librarian scans available BookCopy QR code
        /// 4. Librarian submits confirmation with BookCopy assignments (this endpoint)
        /// 5. System:
        ///    - Updates BookCopy status to Borrowed
        ///    - Creates BookTransaction records
        ///    - Updates BorrowRequest status to Confirmed
        /// 6. Books are now borrowed and tracked
        /// </summary>
        [HttpPost("confirm")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> ConfirmBorrowRequest([FromBody] ConfirmBorrowRequestDto dto)
        {
            try
            {
                // Get the authenticated staff/admin's account ID from JWT
                var accountId = User.GetUserId();
                
                // Confirm the borrow request with specific book copies
                // Staff must provide BookCopyId for each BookId in the request
                var result = await service.ConfirmBorrowRequestAsync(dto, accountId);
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
        /// Admin/Staff rejects borrow request
        /// 
        /// REJECTION FLOW:
        /// - Used when books are not available or member has issues
        /// - Reason is recorded in the request notes
        /// - Member can see rejection reason when viewing their requests
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> RejectBorrowRequest(Guid id, [FromBody] RejectRequestDto dto)
        {
            try
            {
                // Get the authenticated staff/admin's account ID from JWT
                var accountId = User.GetUserId();
                
                var result = await service.RejectBorrowRequestAsync(id, accountId, dto.Reason);
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
        /// 
        /// CANCELLATION FLOW:
        /// - Member can cancel before librarian processes it
        /// - Only pending requests can be cancelled
        /// - If request already confirmed/rejected, cancellation fails
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = Policies.MemberOnly)]
        public async Task<IActionResult> CancelBorrowRequest(Guid id)
        {
            try
            {
                // Get the authenticated member's account ID from JWT
                var accountId = User.GetUserId();
                
                var result = await service.CancelBorrowRequestAsync(id, accountId);
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
        /// Get all pending borrow requests (paged) for admin/staff
        /// 
        /// STAFF DASHBOARD:
        /// - Shows all requests waiting to be processed
        /// - Staff can select and process requests from this list
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetPendingRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await service.GetPendingRequestsPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all borrowed borrow requests (paged) for admin/staff
        /// 
        /// STAFF DASHBOARD:
        /// - Shows all confirmed requests that are currently borrowed and not yet overdue
        /// - Staff can track active borrows
        /// </summary>
        [HttpGet("borrowed")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetBorrowedRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {   
                var result = await service.GetBorrowedRequestsPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all overdue borrow requests (paged) for admin/staff
        /// 
        /// STAFF DASHBOARD:
        /// - Shows all confirmed requests that are past their due date
        /// - Staff can follow up with members who have overdue books
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetOverdueRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await service.GetOverdueRequestsPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all returned borrow requests (returned on time) (paged) for admin/staff
        /// 
        /// STAFF DASHBOARD:
        /// - Shows all requests where all books have been returned on time
        /// - Provides complete borrow history with return dates
        /// - Status: Returned
        /// </summary>
        [HttpGet("returned")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetReturnedRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await service.GetReturnedRequestsPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all overdue returned borrow requests (returned late) (paged) for admin/staff
        /// 
        /// STAFF DASHBOARD:
        /// - Shows all requests where books were returned after the due date
        /// - Helps track members who return books late
        /// - Status: OverdueReturned
        /// </summary>
        [HttpGet("overdue-returned")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetOverdueReturnedRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await service.GetOverdueReturnedRequestsPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get borrow requests for the authenticated member (paged)
        /// 
        /// MEMBER VIEW:
        /// - Member can see all their borrow requests
        /// - Includes: Pending, Borrowed, Overdue, Returned, OverdueReturned, Rejected, Cancelled
        /// - Shows QR codes for pending requests
        /// - Shows borrowed books and due dates for active requests
        /// </summary>
        [HttpGet("my-requests")]
        [Authorize(Policy = Policies.MemberOnly)]
        public async Task<IActionResult> GetMyRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                // Get the authenticated member's account ID from JWT
                var accountId = User.GetUserId();
                
                var result = await service.GetMemberRequestsPagedAsync(accountId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin/Staff gets borrow requests for a specific member (paged)
        /// 
        /// STAFF VIEW:
        /// - View all requests for any member
        /// - Useful for customer service and tracking
        /// </summary>
        [HttpGet("member/{memberId}")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> GetMemberRequests(Guid memberId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await service.GetMemberRequestsByInfoIdPagedAsync(memberId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin/Staff scans a book to return it (individual book return)
        /// 
        /// RETURN FLOW:
        /// 1. Member brings books back to library
        /// 2. Librarian scans BookCopy QR code
        /// 3. System finds active borrow request for that book
        /// 4. Updates BorrowRequest status to Returned/OverdueReturned
        /// 5. Updates BookCopy status to Available
        /// 6. Book is now available for other members
        /// </summary>
        [HttpPost("return")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookDto dto)
        {
            try
            {
                // Get the authenticated staff/admin's account ID from JWT
                var accountId = User.GetUserId();
                
                var result = await service.ReturnBookAsync(dto, accountId);
                if (result.Success)
                    return Ok(new { 
                        message = result.Message, 
                        memberName = result.MemberName, 
                        bookTitle = result.BookTitle,
                        borrowRequestId = result.BorrowRequestId,
                        returnedAt = result.ReturnedAt,
                        wasOverdue = result.WasOverdue
                    });
                else
                    return BadRequest(new { message = result.Message });
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
        public string Reason { get; set; } = string.Empty;
    }
}

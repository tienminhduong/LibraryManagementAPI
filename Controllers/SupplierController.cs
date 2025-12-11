using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize] // Require authentication for all endpoints
    public class SupplierController(ISupplierService supplierService) : ControllerBase
    {
        /// <summary>
        /// Get all suppliers (paginated)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page number and page size must be greater than zero." });
            }

            try
            {
                var suppliers = await supplierService.GetAllSuppliersAsync(pageNumber, pageSize);
                return Ok(suppliers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Search suppliers by name, email, phone number, or address
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchSuppliers([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page number and page size must be greater than zero." });
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term is required." });
            }

            try
            {
                var suppliers = await supplierService.SearchSuppliersAsync(searchTerm, pageNumber, pageSize);
                return Ok(suppliers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add a new supplier (Admin/Staff only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> AddSupplier([FromBody] CreateSupplierDTO supplierDto)
        {
            try
            {
                if (supplierDto == null)
                {
                    return BadRequest(new { message = "Supplier data is null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdSupplier = await supplierService.AddSupplierAsync(supplierDto);
                return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get supplier by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid supplier ID" });
                }

                var supplier = await supplierService.GetSupplierByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound(new { message = "Supplier not found" });
                }
                return Ok(supplier);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update supplier (Admin/Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDTO supplierDto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid supplier ID" });
                }

                if (supplierDto == null)
                {
                    return BadRequest(new { message = "Supplier data is null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await supplierService.UpdateSupplierAsync(id, supplierDto);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete supplier (Admin/Staff only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.StaffOrAdmin)]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid supplier ID" });
            }

            try
            {
                await supplierService.DeleteSupplierAsync(id);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

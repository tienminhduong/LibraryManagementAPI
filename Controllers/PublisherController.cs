using LibraryManagementAPI.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/publishers")]
//[Authorize] 
public class PublisherController(IPublisherService publisherService) : ControllerBase
{
  /// <summary>
  /// Get all publishers (paginated)
  /// </summary>
  [HttpGet]
  public async Task<IActionResult> GetAllPublishers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
  {
    // Validate pagination parameters
    if (pageNumber <= 0 || pageSize <= 0)
    {
      return BadRequest(new { message = "Page number and page size must be greater than zero." });
    }
    // Fetch paginated list of publishers
    try
    {
      var publishers = await publisherService.GetAllPublishersAsync(pageNumber, pageSize);
      return Ok(publishers);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  /// <summary>
  /// Search publishers by name, phone number, or address
  /// </summary>
  [HttpGet("search")]
  public async Task<IActionResult> SearchPublishers([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
  {
    // Validate pagination parameters
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
      var publishers = await publisherService.SearchPublishersAsync(searchTerm, pageNumber, pageSize);
      return Ok(publishers);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  /// <summary>
  /// Add a new publisher (Admin/Staff only)
  /// </summary>
  [HttpPost]
  [Authorize(Policy = Policies.StaffOrAdmin)]
  public async Task<IActionResult> AddPublisher([FromBody] CreatePublisherDTO publisherDto)
  {
    try
    {
      if (publisherDto == null)
      {
        return BadRequest(new { message = "Publisher data is null" });
      }
      var createdPublisher = await publisherService.AddPublisherAsync(publisherDto);
      return CreatedAtAction(nameof(GetPublisherById), new { id = createdPublisher.Id }, createdPublisher);
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
  /// Get publisher by ID
  /// </summary>
  [HttpGet("{id}")]
  public async Task<IActionResult> GetPublisherById(Guid id)
  {
    try
    {
      if (id == Guid.Empty)
      {
        return BadRequest(new { message = "Invalid publisher ID" });
      }

      var publisher = await publisherService.GetPublisherByIdAsync(id);
      if (publisher == null)
      {
        return NotFound(new { message = "Publisher not found" });
      }
      return Ok(publisher);
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
  /// Update publisher (Admin/Staff only)
  /// </summary>
  [HttpPut("{id}")]
  [Authorize(Policy = Policies.StaffOrAdmin)]
  public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] UpdatePublisherDTO publisherDto)
  {
    try
    {
      if (id == Guid.Empty)
      {
        return BadRequest(new { message = "Invalid publisher ID" });
      }

      if (publisherDto == null)
      {
        return BadRequest(new { message = "Publisher data is null" });
      }

      await publisherService.UpdatePublisherAsync(id, publisherDto);
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
  /// Delete publisher (Admin/Staff only)
  /// </summary>
  [HttpDelete("{id}")]
  [Authorize(Policy = Policies.StaffOrAdmin)]
  public async Task<IActionResult> DeletePublisher(Guid id)
  {
    // Validate id and attempt deletion
    if (id == Guid.Empty)
    {
      return BadRequest(new { message = "Invalid publisher ID" });
    }
    // Attempt to delete the publisher
    try
    {
      await publisherService.DeletePublisherAsync(id);
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
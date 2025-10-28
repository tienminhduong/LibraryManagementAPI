using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/publishers")]
public class PublisherController(IPublisherService publisherService) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAllPublishers(int pageNumber = 1, int pageSize = 20)
  {
    // Validate pagination parameters
    if (pageNumber <= 0 || pageSize <= 0)
    {
      return BadRequest("Page number and page size must be greater than zero.");
    }
    // Fetch paginated list of publishers
    try
    {
      var publishers = await publisherService.GetAllPublishersAsync(pageNumber, pageSize);
      return Ok(publishers);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> AddPublisher([FromBody] CreatePublisherDTO publisherDto)
  {
    try
    {
      if (publisherDto == null)
      {
        return BadRequest("Publisher data is null");
      }
      var createdPublisher = await publisherService.AddPublisherAsync(publisherDto);
      return CreatedAtAction(nameof(GetPublisherById), new { id = createdPublisher.Id }, createdPublisher);
    }
    catch (ArgumentNullException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetPublisherById(Guid id)
  {
    try
    {
      if (id == Guid.Empty)
      {
        return BadRequest("Invalid publisher ID");
      }

      var publisher = await publisherService.GetPublisherByIdAsync(id);
      if (publisher == null)
      {
        return NotFound();
      }
      return Ok(publisher);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] UpdatePublisherDTO publisherDto)
  {
    try
    {
      await publisherService.UpdatePublisherAsync(id, publisherDto);
      return NoContent();
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeletePublisher(Guid id)
  {
    // Validate id and attempt deletion
    if (id == Guid.Empty)
    {
      return BadRequest("Invalid publisher ID");
    }
    // Attempt to delete the publisher
    try
    {
      await publisherService.DeletePublisherAsync(id);
      return NoContent();
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
  }
}
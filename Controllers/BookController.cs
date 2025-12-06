using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Utility;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(IBookService bookService,
                            IRecommendationService recommendationService,
                            ILogger logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks([FromQuery] Guid? categoryId ,int pageNumber = 1, int pageSize = 20)
    {
        return Ok(await bookService.GetAllBooksAsync(categoryId, pageNumber, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBookById(Guid id)
    {
        var book = await bookService.GetBookByIdAsync(id);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult> AddBook([FromBody] CreateBookDto bookDto)
    {
        try
        {
            var createdBook = await bookService.AddBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (DbUpdateException exception)
        {
            return Problem(exception.Message);
        }
    }

    [HttpPatch("{id}/categories")]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryOfBookDto dto)
    {
        try
        {
            await bookService.UpdateCategoryOfBookAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{id}/authors")]
    public async Task<ActionResult> UpdateAuthor(Guid id, [FromBody] UpdateAuthorOfBookDto dto)
    {
        try
        {
            await bookService.UpdateAuthorOfBookAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("recommend")]
    public async Task<ActionResult> GetRecommendBooks(int pageNumber = 1, int pageSize = 20)
    {
        // Get Account ID from JWT
        var accountId = User.GetUserId();
        
        var res = await recommendationService.GetRecommendedBooksForUser(accountId, pageNumber, pageSize);
        if(res.isSuccess)
        {
            return Ok(res);
        }
        return BadRequest(res);
    }

    /// <summary>
    /// Tìm kiếm sách theo tên
    /// </summary>
    [HttpGet("search/title")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SearchByTitle(
        [FromQuery] string title,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title cannot be empty");

        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        try
        {
            var result = await bookService.SearchByTitleAsync(title, pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching books by title: {Title}", title);
            return StatusCode(500, "An error occurred while searching");
        }
    }

    /// <summary>
    /// Tìm kiếm sách theo tác giả
    /// </summary>
    [HttpGet("search/author")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SearchByAuthor(
        [FromQuery] string author,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(author))
            return BadRequest("Author cannot be empty");

        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        try
        {
            var result = await bookService.SearchByAuthorAsync(author, pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching books by author: {Author}", author);
            return StatusCode(500, "An error occurred while searching");
        }
    }
}
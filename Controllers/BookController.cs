using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(IBookService bookService,
                            IRecommendationService recommendationService) : ControllerBase
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
}
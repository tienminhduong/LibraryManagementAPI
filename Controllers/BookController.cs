using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks(int pageNumber = 1, int pageSize = 20)
    {
        return Ok(await bookService.GetAllBooksAsync(pageNumber, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBookById(Guid id)
    {
        var book = await bookService.GetBookByIdAsync(id);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult> AddBook([FromBody] CreateBookDTO bookDto)
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
}
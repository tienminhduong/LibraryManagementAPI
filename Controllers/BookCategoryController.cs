using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BookCategory;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/book-categories")]
public class BookCategoryController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCategoryDto>>> GetAllBookCategories()
    {
        return Ok(await bookService.GetAllBookCategoriesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookCategoryDto>> GetBookCategoryById(Guid id)
    {
        var category = await bookService.GetBookCategoryByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpGet("{id}/books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        var books = await bookService.GetAllBooksInCategoryAsync(id, pageNumber, pageSize);
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookCategory([FromBody] CreateBookCategoryDto categoryDto)
    {
        try
        {
            var createdCategory = await bookService.CreateBookCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetBookCategoryById), new { id = createdCategory.Id }, createdCategory);
        }
        catch (ExistedException)
        {
            return NoContent();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBookCategory(Guid id, [FromBody] UpdateBookCategoryDto categoryDto)
    {
        try
        {
            await bookService.UpdateBookCategoryAsync(id, categoryDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBookCategory(Guid id)
    {
        try
        {
            await bookService.DeleteBookCategoryAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
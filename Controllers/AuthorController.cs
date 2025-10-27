using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddNewAuthor([FromBody] CreateAuthorDto authorDto)
    {
        var createdAuthor = await authorService.AddNewAuthor(authorDto);
        return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.Id }, createdAuthor);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorById(Guid id)
    {
        try
        {
            return await authorService.GetAuthorAsync(id);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AuthorDto>>> GetAllAuthors(int pageNumber = 1, int pageSize = 20)
    {
        return Ok(await authorService.GetAllAuthorsAsync(pageNumber, pageSize));
    }

    [HttpGet("{id}/books")]
    public async Task<ActionResult> GetBooksByAuthor(Guid id, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var books = await authorService.GetAllBooksByAuthorAsync(id, pageNumber, pageSize);
            return Ok(books);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuthor(Guid id, [FromBody] UpdateAuthorDto authorDto)
    {
        try
        {
            await authorService.UpdateAuthorAsync(id, authorDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuthor(Guid id)
    {
        try
        {
            await authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
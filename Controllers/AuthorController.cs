using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Author;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/authors")]
[Produces("application/json")]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddNewAuthor([FromBody] CreateAuthorDto authorDto)
    {
        var createdAuthor = await authorService.AddNewAuthor(authorDto);
        return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.Id }, createdAuthor);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(PagedResponse<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AuthorDto>>> GetAllAuthors(int pageNumber = 1, int pageSize = 20)
    {
        return Ok(await authorService.GetAllAuthorsAsync(pageNumber, pageSize));
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchAuthor(
        string? nameQuery = null,
        int? yearOfBirth = null,
        int? yearOfBirthBefore = null,
        int? yearOfBirthAfter = null,
        string? briefDescriptionContains = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var authors = await authorService.SearchAuthor(nameQuery, yearOfBirth, yearOfBirthBefore, yearOfBirthAfter, briefDescriptionContains, pageNumber, pageSize);
        return Ok(authors);
    }

    [HttpGet("{id}/books")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
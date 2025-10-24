using System.Security.Cryptography.X509Certificates;
using API.Entities;
using API.Interfaces;
using API.Models;
using AutoMapper;
using LibraryManagementAPI.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCategoryController(
    IBookCategoryRepository bookCategoryRepository,
    IBookService bookService
    ) : ControllerBase
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
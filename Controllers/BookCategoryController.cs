using API.Entities;
using API.Interfaces;
using AutoMapper;
using LibraryManagementAPI.Models.BookCategory;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCategoryController(
    IBookCategoryRepository bookCategoryRepository,
    IMapper mapper
    ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCategoryDto>>> GetAllBookCategories()
    {
        var categories = await bookCategoryRepository.GetAllCategories();
        return Ok(mapper.Map<IEnumerable<BookCategoryDto>>(categories));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookCategoryDto>> GetBookCategoryById(Guid id)
    {
        var category = await bookCategoryRepository.GetCategoryById(id);
        if (category == null) return NotFound();
        //return Ok(mapper.Map<BookCategoryDto>(category));
        return Ok("Hello");
    }
    [HttpPost]
    public async Task<ActionResult> CreateBookCategory([FromBody] CreateBookCategoryDto categoryDto)
    {
        //Check valid data
        if (ModelState.IsValid == false) return BadRequest(ModelState);
        // check existing category
        var isExisting = await bookCategoryRepository.IsCategoryExistsByName(categoryDto.Name);
        if (isExisting) 
            return Conflict("Category with the same name already exists");
        // Create new category
        var category = mapper.Map<BookCategory>(categoryDto);
        // Save to database
        var result = await bookCategoryRepository.AddCategory(category);
        // Return response if failed
        if (!result) 
            return BadRequest("Failed to create category");
        // Return created response
        var createdCategoryDto = mapper.Map<BookCategoryDto>(category);
        return CreatedAtAction(nameof(GetBookCategoryById), new { id = createdCategoryDto.Id }, createdCategoryDto);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBookCategory(Guid id, [FromBody] BookCategoryDto categoryDto)
    {
        //Check valid data
        if (id == Guid.Empty) return BadRequest("Invalid ID");
        if (ModelState.IsValid == false) return BadRequest(ModelState);
        if (id != categoryDto.Id) return BadRequest("ID mismatch");
        //
        var existingCategory = await bookCategoryRepository.GetCategoryById(id);
        if (existingCategory == null) return NotFound();

        mapper.Map(categoryDto, existingCategory);
        var result = await bookCategoryRepository.UpdateCategory(existingCategory);
        if (!result) return BadRequest("Failed to update category");
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBookCategory(Guid id)
    {
        var existingCategory = await bookCategoryRepository.GetCategoryById(id);
        if (existingCategory == null) return NotFound();
        var result = await bookCategoryRepository.DeleteCategory(id);
        if (!result) return BadRequest("Failed to delete category");
        return NoContent();
  }
}
using API.Entities;
using API.Interfaces;
using API.Models;
using AutoMapper;
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
    public async Task<ActionResult<IEnumerable<BookCategory>>> GetAllBookCategories()
    {
        var categories = await bookCategoryRepository.GetAllCategories();
        return Ok(mapper.Map<IEnumerable<BookCategoryDto>>(categories));
    }
}
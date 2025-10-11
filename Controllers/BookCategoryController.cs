using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCategoryController(IBookCategoryRepository bookCategoryRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCategory>>> GetAllBookCategories()
    {
        var categories = await bookCategoryRepository.GetAllCategories();
        return Ok(categories);
    }
}
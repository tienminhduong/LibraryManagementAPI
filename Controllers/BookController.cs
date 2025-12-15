using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Exceptions;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.Utility;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(IBookService bookService,
                            IRecommendationService recommendationService,
                            ILogger logger,
                            IInfoRepository infoRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks([FromQuery] Guid? categoryId, int pageNumber = 1, int pageSize = 20)
    {
        return Ok(await bookService.GetAllBooksAsync(categoryId, pageNumber, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBookById(Guid id)
    {
        var book = await bookService.GetBookByIdAsync(id);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpGet("search")]
    [OutputCache(Duration = 60, VaryByRouteValueNames = ["titleQuery", "authorName", "categoryName", "publisherName", "isbn", "publishedYear", "descriptionContains", "pageNumber", "pageSize"])]
    public async Task<ActionResult> SearchBooks(
        string? isbn = null,
        string? titleQuery = null,
        string? categoryName = null,
        string? authorName = null,
        string? publisherName = null,
        int? publishedYear = null,
        string? descriptionContains = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var books = await bookService.SearchBooks(isbn, titleQuery, categoryName, authorName, publisherName, publishedYear, descriptionContains, pageNumber, pageSize);
        return Ok(books);
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

    [HttpPost("import")]
    [Authorize(Policy = Policies.StaffOrAdmin)]
    public async Task<ActionResult> ImportBooks([FromBody] CreateBookImportDto createBookImportDto)
    {
        try
        {
            var accountId = User.GetUserId();
            var role = User.GetUserRole();
            if (role != "Staff")
                return Unauthorized("Only staff can import books");

            var staffInfo = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Staff);
            if (staffInfo == null)
                return Unauthorized("Staff info not found");

            var staffId = staffInfo.id;
            var id = await bookService.ImportBooks(createBookImportDto, staffId);
            // Use named route to ensure URL generation succeeds
            return CreatedAtRoute("GetBookImportById", new { id }, null);
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

    [HttpGet("import/{id}", Name = "GetBookImportById")]
    public async Task<ActionResult> GetBookImportById(Guid id)
    {
        var bookImport = await bookService.GetImportHistoryByIdAsync(id);
        if (bookImport == null)
            return NotFound($"No import history with Id: {id}");
        return Ok(bookImport);
    }

    [HttpGet("import")]
    public async Task<ActionResult> GetImportHistory(
        string? supplierName = null,
        string? staffName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1, int pageSize = 20)
    {
        var histroy =
            await bookService.GetImportHistoryAsync(supplierName, staffName, startDate, endDate, pageNumber, pageSize);
        return Ok(histroy);
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
    public async Task<ActionResult> GetRecommendBooks(int pageNumber = 1, int pageSize = 20, float alpha = 0.6f)
    {
        // Get Account ID from JWT
        var accountId = User.GetUserId();
        var type = User.GetUserRole();
        var info = await infoRepository.GetInfoByAccountIdAsync(accountId, Role.Member);
        var memberId = info?.id;

        var res = await recommendationService.GetRecommendedBooksForUser(memberId, pageNumber, pageSize, alpha);
        if (res != null)
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

    [HttpGet("{id}/copies/qrs")]
    public async Task<ActionResult> GetBookCopiesQrCodes(Guid id)
    {
        try
        {
            var copies = await bookService.GetCopiesByBookIdAsync(id);
            if (copies == null || !copies.Any())
                return NotFound(new { message = "No copies found for this book." });

            var result = copies.Select(c => new {
                CopyId = c.id,
                QrCode = c.QrCode,
                Status = c.status.ToString()
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting book copy QR codes for book {BookId}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("top-books")]
    public async Task<IActionResult> GetTopBooks(DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var topBooks = await bookService.GetTopBookByTimeAsync(from, to, pageNumber, pageSize);
            
            if(topBooks != null)
            {
                return Ok(topBooks);
            }
            else
            {
                return BadRequest(topBooks);
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
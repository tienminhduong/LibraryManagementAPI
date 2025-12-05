using LibraryManagementAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Examples;

/// <summary>
/// Example usage of BookCopy QR code generation
/// </summary>
public class BookCopyQrCodeExamples
{
    /*
     * EXAMPLE 1: Using the entity's computed property
     * ================================================
     */
    public void Example1_UsingEntityProperty()
    {
        var bookCopy = new Entities.BookCopy 
        { 
            id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
            bookId = Guid.NewGuid(),
            bookImportDetailId = Guid.NewGuid(),
            status = Entities.Status.Available
        };
        
        // QR code is automatically available via the computed property
        string qrCode = bookCopy.QrCode;  // Returns: "COPY-12345678-1234-1234-1234-123456789012"
    }

    /*
     * EXAMPLE 2: Using the extension method
     * ======================================
     */
    public void Example2_UsingExtensionMethod()
    {
        Guid bookCopyId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        
        // Generate QR code from any BookCopyId
        string qrCode = bookCopyId.GenerateQrCode();  // Returns: "COPY-12345678-1234-1234-1234-123456789012"
        
        // Extract BookCopyId back from QR code
        Guid? extractedId = Extensions.BookCopyExtensions.ExtractBookCopyId(qrCode);
    }

    /*
     * EXAMPLE 3: Using repository methods
     * ====================================
     */
    public async Task Example3_UsingRepository(Interfaces.IRepositories.IBookCopyRepository repo)
    {
        Guid bookCopyId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        
        // Generate QR code
        string qrCode = repo.GenerateQrCode(bookCopyId);
        
        // Find BookCopy by QR code (when scanning)
        var bookCopy = await repo.GetByQrCode(qrCode);
        if (bookCopy != null)
        {
            Console.WriteLine($"Found: {bookCopy.book?.Title}");
        }
    }
}

/*
 * EXAMPLE 4: In a controller for scanning
 * =========================================
 * This is a real controller example that you can use in your Controllers folder
 */
/*
[ApiController]
[Route("api/[controller]")]
public class BookCopyController : ControllerBase
{
    private readonly IBookCopyRepository _bookCopyRepo;

    public BookCopyController(IBookCopyRepository bookCopyRepo)
    {
        _bookCopyRepo = bookCopyRepo;
    }

    // GET /api/bookcopy/scan?qrcode=COPY-12345678-1234-1234-1234-123456789012
    [HttpGet("scan")]
    public async Task<IActionResult> ScanQrCode([FromQuery] string qrcode)
    {
        var bookCopy = await _bookCopyRepo.GetByQrCode(qrcode);
        
        if (bookCopy == null)
            return NotFound(new { message = "Book copy not found" });
        
        return Ok(new 
        { 
            id = bookCopy.id,
            bookId = bookCopy.bookId,
            bookTitle = bookCopy.book?.Title,
            status = bookCopy.status.ToString(),
            qrCode = bookCopy.QrCode  // Using computed property
        });
    }

    // GET /api/bookcopy/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookCopy(Guid id)
    {
        try
        {
            var bookCopy = await _bookCopyRepo.GetById(id);
            
            return Ok(new 
            { 
                id = bookCopy.id,
                bookId = bookCopy.bookId,
                status = bookCopy.status.ToString(),
                qrCode = bookCopy.QrCode  // QR code is always available
            });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    // GET /api/bookcopy/available/{bookId}
    [HttpGet("available/{bookId}")]
    public async Task<IActionResult> GetAvailableCopies(Guid bookId)
    {
        var copies = await _bookCopyRepo.GetAvailableCopiesByBookId(bookId);
        
        var result = copies.Select(c => new
        {
            id = c.id,
            bookId = c.bookId,
            status = c.status.ToString(),
            qrCode = c.QrCode
        });
        
        return Ok(result);
    }
}
*/

using API.Interfaces;
using AutoMapper;
using LibraryManagementAPI.Interfaces;
using LibraryManagementAPI.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookRepository bookRepository,
                                IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
        {
            var books = await bookRepository.GetAllBooks();
            return Ok(mapper.Map<IEnumerable<BookDTO>>(books));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBookById(Guid id)
        {
            // Validate the ID
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid book ID");
            }
            // Fetch the book from the repository
            var book = await bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<BookDTO>(book));
        }
        [HttpPost]
        public async Task<ActionResult> AddBook([FromBody] CreateBookDTO bookDto)
        {
            // Validate the input
            if (bookDto == null)
            {
                return BadRequest("Book data is null");
            }
            // Model state validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var book = mapper.Map<API.Entities.Book>(bookDto);
            var result = await bookRepository.AddBook(book);
            if (!result)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, mapper.Map<BookDTO>(book));
        }
    }
}

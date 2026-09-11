using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BokApi.Models;
using BokApi.Data;

namespace BokApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return Ok(await _context.Books.ToListAsync());
        }

        // GET: api/books/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book newBook)
        {
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }

        // PUT: api/books/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublishedDate = updatedBook.PublishedDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
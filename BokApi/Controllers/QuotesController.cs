using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BokApi.Models;
using BokApi.Data;

namespace BokApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuotesController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUsername()
        {
            return User.FindFirstValue(ClaimTypes.Name)!;
        }

        private async Task<int> GetUserIdAsync()
        {
            var username = GetUsername();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user!.Id;
        }

        // GET: api/quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
        {
            int userId = await GetUserIdAsync();
            var quotes = await _context.Quotes.Where(q => q.UserId == userId).ToListAsync();
            return Ok(quotes);
        }

        // GET: api/quotes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            int userId = await GetUserIdAsync();
            var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        // POST: api/quotes
        [HttpPost]
        public async Task<ActionResult<Quote>> CreateQuote(Quote newQuote)
        {
            newQuote.UserId = await GetUserIdAsync();
            _context.Quotes.Add(newQuote);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuote), new { id = newQuote.Id }, newQuote);
        }

        // PUT: api/quotes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(int id, Quote updatedQuote)
        {
            int userId = await GetUserIdAsync();
            var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quote == null)
                return NotFound();

            quote.Text = updatedQuote.Text;
            quote.Author = updatedQuote.Author;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/quotes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            int userId = await GetUserIdAsync();
            var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quote == null)
                return NotFound();

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
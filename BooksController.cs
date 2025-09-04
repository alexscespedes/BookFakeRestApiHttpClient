using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BookApiHttpClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("BookFakeApi");

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksAsync()
        {
            using var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("Books");

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var books = await JsonSerializer.DeserializeAsync<IEnumerable<Book>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
        {
            using var httpClient = CreateClient();
            using var httpResponseMessage = await httpClient.GetAsync($"Books/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var book = await JsonSerializer.DeserializeAsync<Book>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBookAsync(Book book)
        {
            using var httpClient = CreateClient();
            using var httpResponseMessage = await httpClient.PostAsJsonAsync("Books", book);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            var createdBook = await httpResponseMessage.Content.ReadFromJsonAsync<Book>();

            if (createdBook == null)
                return Problem("the API did not return a created book");

            return Created(string.Empty, book);

            // return CreatedAtAction(nameof(GetBookByIdAsync), new { id = book?.Id }, createdBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest("Book ID mismatch");

            using var httpClient = CreateClient();
            using var httpResponseMessage = await httpClient.PutAsJsonAsync($"Books/{id}", book);

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            using var httpClient = CreateClient();
            using var httpResponseMessage = await httpClient.DeleteAsync($"Books/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            return NoContent();
        }
    }
}

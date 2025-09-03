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

        public IEnumerable<Book>? Books { get; set; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            // var response = await client.GetAsync("");
            var httpResponseMessage = await httpClient.GetAsync("Books");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                Books = await JsonSerializer.DeserializeAsync<IEnumerable<Book>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            return Ok(Books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            var httpResponseMessage = await httpClient.GetAsync($"Books/{id}");

            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var book = await JsonSerializer.DeserializeAsync<Book>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            httpResponseMessage.EnsureSuccessStatusCode();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBookAsync(Book book)
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            var bookJson = new StringContent(JsonSerializer.Serialize(book),
            Encoding.UTF8,
            Application.Json);

            using var httpResponseMessage =
                await httpClient.PostAsync("Books", bookJson);

            httpResponseMessage.EnsureSuccessStatusCode();

            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(Book book)
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            var bookJson = new StringContent(JsonSerializer.Serialize(book),
            Encoding.UTF8,
            Application.Json);

            using var httpResponseMessage =
                await httpClient.PutAsync($"Books/{book.Id}", bookJson);

            httpResponseMessage.EnsureSuccessStatusCode();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            using var httpResponseMessage =
                await httpClient.DeleteAsync($"Books/{id}");

            httpResponseMessage.EnsureSuccessStatusCode();

            return NoContent();
        }

    }
}

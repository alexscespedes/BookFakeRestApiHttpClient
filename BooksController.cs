using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            // var response = await client.GetAsync("");
            var httpResponseMessage = await httpClient.GetAsync("Books");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                Books = await JsonSerializer.DeserializeAsync<IEnumerable<Book>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }

            return Books!;
        }

        [HttpGet("{id}")]
        public async Task<Book> GetBookByIdAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("BookFakeApi");

            var httpResponseMessage = await httpClient.GetAsync($"Books/{id}");

            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var book = await JsonSerializer.DeserializeAsync<Book>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            httpResponseMessage.EnsureSuccessStatusCode();

            return book!;
        }

    }
}

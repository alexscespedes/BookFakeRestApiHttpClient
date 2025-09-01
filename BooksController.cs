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
            var client = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, "https://fakerestapi.azurewebsites.net/api/v1/Books"
            );

            // var response = await client.GetAsync("");
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                Books = await JsonSerializer.DeserializeAsync<IEnumerable<Book>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }

            return Books!;
        }
    }
}

using Microsoft.Extensions.Logging;

namespace DTTestTask.Services
{
    public interface ICsvService
    {
        Task<Stream> ReadCsvUrlAsync(string url);
    }

    public class CsvService : ICsvService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CsvService> _logger;

        public CsvService(IHttpClientFactory httpClientFactory, ILogger<CsvService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<Stream> ReadCsvUrlAsync(string url)
        {
            var client = _httpClientFactory.CreateClient();

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

    }
}

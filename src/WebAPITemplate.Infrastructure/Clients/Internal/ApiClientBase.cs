using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace WebAPITemplate.Infrastructure.Clients.Internal
{
    public class ApiClientBase
    {
        private const string _className = "ApiClientBase";
        protected readonly HttpClient _httpClient;
        private readonly ILogger<ApiClientBase> _logger;

        public ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected async Task<List<T>> GetAllAsync<T>(string endpoint)
        {
            _logger.LogDebug($"{_className} - Calling endpoint: {_httpClient.BaseAddress + endpoint}");
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<T>>();
            _logger.LogDebug($"{_className} - Request Success!");
            return result ?? new List<T>();
        }

        protected async Task<T?> GetByIdAsync<T>(string endpoint)
        {
            _logger.LogDebug($"{_className} - Calling endpoint: {_httpClient.BaseAddress + endpoint}");
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            _logger.LogDebug($"{_className} - Request Success!");
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}

using WebAPITemplate.Application.Interfaces.Services;
using System.Diagnostics.Metrics;
using WebAPITemplate.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;

namespace WebAPITemplate.Application.Services
{
    public class Service : IService
    {
        private const string className = "TestService";
        private readonly ILogger<Service> _logger;
        private readonly IApiClient _apiClient;

        public Service(ILogger<Service> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<List<object>> GetAllAsync()
        {
            _logger.LogDebug("GetAllAsync");
            var obj = await _apiClient.GetAllAsync();
            return obj;
        }

        public async Task<object> GetByIdAsync(long id)
        {
            _logger.LogDebug("GetByIdAsync");
            var obj = await _apiClient.GetByIdAsync(id);
            return obj;
        }
    }
}

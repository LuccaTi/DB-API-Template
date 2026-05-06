using Microsoft.Extensions.Logging;
using WebAPITemplate.Application.Interfaces.Clients;
using WebAPITemplate.Infrastructure.Clients.Internal;

namespace WebAPITemplate.Infrastructure.Clients
{
    public class ApiClient : ApiClientBase, IApiClient
    {
        private const string _className = "ApiClient";
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger) : base(httpClient, logger)
        {
            _logger = logger;
        }

        public async Task<List<object>> GetAllAsync()
        {
            var obj = await base.GetAllAsync<object>("users");
            return obj;
        }

        public async Task<object> GetByIdAsync(long id)
        {
            var obj = await base.GetByIdAsync<object>($"users/{id}");
            if (obj == null)
                obj = new object();

            return obj;
        }
    }
}

namespace WebAPITemplate.Application.Interfaces.Clients;

public interface IApiClient
{
    public Task<List<object>> GetAllAsync();
    public Task<object> GetByIdAsync(long id);
}

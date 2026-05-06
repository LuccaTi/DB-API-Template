namespace WebAPITemplate.Application.Interfaces.Services;

public interface IService
{
    public Task<List<object>> GetAllAsync();
    public Task<object> GetByIdAsync(long id);
}

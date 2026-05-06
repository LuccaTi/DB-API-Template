using WebAPITemplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace APITemplate.Host.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IService _service;

        public TestController(IService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var message = await _service.GetAllAsync();
            return Ok(message);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var message = await _service.GetByIdAsync(id);
            return Ok(message);
        }
    }
}

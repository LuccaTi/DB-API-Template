using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace DBAPITemplate.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtRoute(nameof(GetByIdAsync), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] ProductRequestDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleleAsync(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}

using DBAPITemplate.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAPITemplate.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(long id);
        Task<ProductResponseDto> CreateAsync(ProductRequestDto dto);
        Task UpdateAsync(long id, ProductRequestDto dto);
        Task DeleteAsync(long id);
    }
}

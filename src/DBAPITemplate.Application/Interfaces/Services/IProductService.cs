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
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductResponseDto> GetByIdAsync(GetProductByIdRequestDto request, CancellationToken cancellationToken = default);
        Task<ProductResponseDto> CreateAsync(ProductRequestDto dto, CancellationToken cancellationToken = default);
        Task UpdateAsync(long id, ProductRequestDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
}

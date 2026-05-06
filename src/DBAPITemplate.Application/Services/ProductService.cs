using MapsterMapper;
using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Application.Interfaces.Repositories;
using DBAPITemplate.Application.Interfaces.Services;
using DBAPITemplate.Domain.Entities;
using DBAPITemplate.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAPITemplate.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> GetByIdAsync(long id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto dto)
        {
            var entity = _mapper.Map<Product>(dto);
            var createdEntity = await _repository.CreateAsync(entity);
            return _mapper.Map<ProductResponseDto>(createdEntity);
        }

        public async Task UpdateAsync(long id, ProductRequestDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");

            _mapper.Map(dto, product);
            await _repository.UpdateAsync(product);
        }

        public async Task DeleteAsync(long id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");

            await _repository.DeleteAsync(id);
        }

    }
}

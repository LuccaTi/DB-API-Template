using MapsterMapper;
using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Application.Extensions;
using DBAPITemplate.Application.Interfaces.Repositories;
using DBAPITemplate.Application.Interfaces.Services;
using DBAPITemplate.Domain.Entities;
using DBAPITemplate.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using FluentValidation;

namespace DBAPITemplate.Application.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        private readonly IValidator<GetProductByIdRequestDto> _getByIdValidator;
        private readonly IValidator<ProductRequestDto> _requestValidator;

        public ProductService(
            IProductRepository repository, 
            IMapper mapper, 
            IValidator<GetProductByIdRequestDto> getValidator, 
            IValidator<ProductRequestDto> requestValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _getByIdValidator = getValidator;
            _requestValidator = requestValidator;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> GetByIdAsync(GetProductByIdRequestDto request, CancellationToken cancellationToken = default)
        {
            await _getByIdValidator.ValidateAndThrowCustomAsync(request, cancellationToken);

            var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {request.Id} not found.");

            return _mapper.Map<ProductResponseDto>(product);
        }

        private async Task<bool> EntityExists(string name, CancellationToken cancellationToken = default)
        {
            var product = await _repository.GetByNameAsync(name, cancellationToken);
            if(product == null)
                return false;

            return true;
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto dto, CancellationToken cancellationToken = default)
        {
            await _requestValidator.ValidateAndThrowCustomAsync(dto, cancellationToken);

            var entityExists = await EntityExists(dto.Name, cancellationToken);
            if (entityExists)
                throw new ConflictException($"Product '{dto.Name}' already exists.");

            var entity = _mapper.Map<DBAPITemplate.Domain.Entities.Product>(dto);

            var createdEntity = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<ProductResponseDto>(createdEntity);
        }

        public async Task UpdateAsync(long id, ProductRequestDto dto, CancellationToken cancellationToken = default)
        {
            await _requestValidator.ValidateAndThrowCustomAsync(dto, cancellationToken);

            var product = await _repository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");

            _mapper.Map(dto, product);
            await _repository.UpdateAsync(product, cancellationToken);
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var product = await _repository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");

            await _repository.DeleteAsync(id, cancellationToken);
        }

    }
}

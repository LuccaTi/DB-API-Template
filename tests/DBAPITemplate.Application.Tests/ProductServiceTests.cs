using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Application.Interfaces.Repositories;
using DBAPITemplate.Application.Services;
using DBAPITemplate.Domain.Entities;
using DBAPITemplate.Domain.Exceptions;
using MapsterMapper;
using Moq;

namespace DBAPITemplate.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ProductService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProductResponseDto()
    {
        // 1.ARRANGE
        long productId = 1;
        var productEntity = new Product { Id = productId, Name = "Keyboard", Price = 150 };
        var productDto = new ProductResponseDto { Id = productId, Name = "Keyboard", Price = 150 };

        _repositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(productEntity);

        _mapperMock.Setup(mapper => mapper.Map<ProductResponseDto>(productEntity))
            .Returns(productDto);

        // 2.ACT
        var result = await _service.GetByIdAsync(productId);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Keyboard", result.Name);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldThrowNotFoundException()
    {
        // 1.ARRANGE
        long invalidProductId = 99;

        _repositoryMock.Setup(repo => repo.GetByIdAsync(invalidProductId))
            .ReturnsAsync((Product?)null);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(invalidProductId));

        Assert.Equal($"Product with ID {invalidProductId} not found.", exception.Message);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(invalidProductId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExists_ShouldReturnListOfProductResponseDto()
    {
        // 1.ARRANGE
        var productEntities = new List<Product>
        {
            new Product { Id = 1, Name = "Keyboard", Price = 150},
            new Product { Id = 2, Name = "Mouse", Price = 80}
        };

        var productDtos = new List<ProductResponseDto>
        {
            new ProductResponseDto {Id = 1, Name = "Keyboard", Price = 150},
            new ProductResponseDto {Id = 2, Name = "Mouse", Price = 80}
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(productEntities);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductResponseDto>>(productEntities))
            .Returns(productDtos);

        // 2.ACT
        var result = await _service.GetAllAsync();

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        var resultList = result.ToList();
        Assert.Equal("Keyboard", resultList[0].Name);
        Assert.Equal("Mouse", resultList[1].Name);

        _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        // 1.ARRANGE
        var emptyProductEntities = new List<Product>();
        var emptyProductDtos = new List<ProductResponseDto>();

        _repositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(emptyProductEntities);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductResponseDto>>(emptyProductEntities))
            .Returns(emptyProductDtos);

        // 2.ACT
        var result = await _service.GetAllAsync();

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Empty(result);

        _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenProductNameDoesNotExist_ShouldCreateProduct()
    {
        // 1.ARRANGE
        var entityRequestDto = new ProductRequestDto { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var mappedEntity = new Product { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var savedEntity = new Product { Id = 1, Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var responseDto = new ProductResponseDto { Id = 1, Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        _repositoryMock.Setup(repo => repo.GetByNameAsync(entityRequestDto.Name))
            .ReturnsAsync((Product?)null);

        _mapperMock.Setup(mapper => mapper.Map<Product>(entityRequestDto))
            .Returns(mappedEntity);

        _repositoryMock.Setup(repo => repo.CreateAsync(mappedEntity))
            .ReturnsAsync(savedEntity);

        _mapperMock.Setup(mapper => mapper.Map<ProductResponseDto>(savedEntity))
            .Returns(responseDto);

        // 2.ACT
        var result = await _service.CreateAsync(entityRequestDto);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Keyboard", result.Name);

        _repositoryMock.Verify(repo => repo.GetByNameAsync(entityRequestDto.Name), Times.Once);
        _repositoryMock.Verify(repo => repo.CreateAsync(mappedEntity), Times.Once);

    }

    [Fact]
    public async Task CreateAsync_WhenProductNameExist_ShouldThrowConflictException()
    {
        // 1.ARRANGE
        var entityRequestDto = new ProductRequestDto { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };
        var existingProductInDb = new Product { Id = 1, Name = "Keyboard", Price = 15, Description = "Pc Keyboard" };

        _repositoryMock.Setup(repo => repo.GetByNameAsync(entityRequestDto.Name))
            .ReturnsAsync(existingProductInDb);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(entityRequestDto));

        Assert.Equal($"Product '{entityRequestDto.Name}' already exists.", exception.Message);

        _repositoryMock.Verify(repo => repo.GetByNameAsync(entityRequestDto.Name), Times.Once);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
    }
}
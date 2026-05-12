using DBAPITemplate.Application.DTOs.Product;
using DBAPITemplate.Application.Interfaces.Repositories;
using DBAPITemplate.Application.Services.Product;
using DBAPITemplate.Domain.Entities;
using DBAPITemplate.Domain.Exceptions;
using FluentValidation;
using MapsterMapper;
using Moq;

namespace DBAPITemplate.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<GetProductByIdRequestDto>> _mockGetValidator;
    private readonly Mock<IValidator<ProductRequestDto>> _mockRequestValidator;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _mockGetValidator = new Mock<IValidator<GetProductByIdRequestDto>>();
        _mockRequestValidator = new Mock<IValidator<ProductRequestDto>>();

        _service = new ProductService(
            _repositoryMock.Object, 
            _mapperMock.Object, 
            _mockGetValidator.Object, 
            _mockRequestValidator.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExistsAndRequestIsValid_ShouldReturnProductResponseDto()
    {
        // 1.ARRANGE
        var request = new GetProductByIdRequestDto { Id = 1 };
        var productEntity = new Product { Id = 1, Name = "Keyboard", Price = 150 };
        var productDto = new ProductResponseDto { Id = 1, Name = "Keyboard", Price = 150 };

        // CancellationToken.None is used to simulate a http request that has not been cancelled.
        var cancellationToken = CancellationToken.None;

        _mockGetValidator.Setup(v => v.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _repositoryMock.Setup(repo => repo.GetByIdAsync(request.Id, cancellationToken))
            .ReturnsAsync(productEntity);

        _mapperMock.Setup(mapper => mapper.Map<ProductResponseDto>(productEntity))
            .Returns(productDto);

        // 2.ACT
        var result = await _service.GetByIdAsync(request, cancellationToken);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(request.Id, result.Id);
        Assert.Equal("Keyboard", result.Name);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRequestIsInvalid_ShouldThrowValidationException()
    {
        // 1.ARRANGE
        var request = new GetProductByIdRequestDto { Id = -5 };

        var cancellationToken = CancellationToken.None;

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Id", "Id must be greater than zero.")
        };

        var invalidResult = new FluentValidation.Results.ValidationResult(validationFailures);

        _mockGetValidator
            .Setup(v => v.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(invalidResult);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(request, cancellationToken));

        Assert.NotEmpty(exception.Errors);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<long>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExistAndRequestIsValid_ShouldThrowNotFoundException()
    {
        // 1.ARRANGE
        var request = new GetProductByIdRequestDto { Id = 99 };

        var cancellationToken = CancellationToken.None;

        var validResult = new FluentValidation.Results.ValidationResult();

        _mockGetValidator
            .Setup(v => v.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(validResult);

        _repositoryMock.Setup(repo => repo.GetByIdAsync(request.Id, cancellationToken))
            .ReturnsAsync((Product?)null);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(request, cancellationToken));

        Assert.Equal($"Product with ID {request.Id} not found.", exception.Message);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
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

        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(productEntities);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductResponseDto>>(productEntities))
            .Returns(productDtos);

        // 2.ACT
        var result = await _service.GetAllAsync(cancellationToken);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        var resultList = result.ToList();
        Assert.Equal("Keyboard", resultList[0].Name);
        Assert.Equal("Mouse", resultList[1].Name);

        _repositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        // 1.ARRANGE
        var emptyProductEntities = new List<Product>();
        var emptyProductDtos = new List<ProductResponseDto>();

        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(repo => repo.GetAllAsync(cancellationToken))
            .ReturnsAsync(emptyProductEntities);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductResponseDto>>(emptyProductEntities))
            .Returns(emptyProductDtos);

        // 2.ACT
        var result = await _service.GetAllAsync(cancellationToken);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Empty(result);

        _repositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenProductNameDoesNotExist_ShouldCreateProduct()
    {
        // 1.ARRANGE
        var entityRequestDto = new ProductRequestDto { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var mappedEntity = new Product { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var savedEntity = new Product { Id = 1, Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var responseDto = new ProductResponseDto { Id = 1, Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };

        var cancellationToken = CancellationToken.None;

        var validResult = new FluentValidation.Results.ValidationResult();

        _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<ProductRequestDto>(), cancellationToken))
            .ReturnsAsync(validResult);

        _repositoryMock.Setup(repo => repo.GetByNameAsync(entityRequestDto.Name, cancellationToken))
            .ReturnsAsync((Product?)null);

        _mapperMock.Setup(mapper => mapper.Map<Product>(entityRequestDto))
            .Returns(mappedEntity);

        _repositoryMock.Setup(repo => repo.CreateAsync(mappedEntity, cancellationToken))
            .ReturnsAsync(savedEntity);

        _mapperMock.Setup(mapper => mapper.Map<ProductResponseDto>(savedEntity))
            .Returns(responseDto);

        // 2.ACT
        var result = await _service.CreateAsync(entityRequestDto, cancellationToken);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Keyboard", result.Name);

        _repositoryMock.Verify(repo => repo.GetByNameAsync(entityRequestDto.Name, cancellationToken), Times.Once);
        _repositoryMock.Verify(repo => repo.CreateAsync(mappedEntity, cancellationToken), Times.Once);

    }

    [Fact]
    public async Task CreateAsync_WhenProductNameExist_ShouldThrowConflictException()
    {
        // 1.ARRANGE
        var entityRequestDto = new ProductRequestDto { Name = "Keyboard", Price = 10, Description = "Pc Keyboard" };
        var existingProductInDb = new Product { Id = 1, Name = "Keyboard", Price = 15, Description = "Pc Keyboard" };

        var cancellationToken = CancellationToken.None;

        var validResult = new FluentValidation.Results.ValidationResult();

        _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<ProductRequestDto>(), cancellationToken))
            .ReturnsAsync(validResult);

        _repositoryMock.Setup(repo => repo.GetByNameAsync(entityRequestDto.Name, cancellationToken))
            .ReturnsAsync(existingProductInDb);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(entityRequestDto, cancellationToken));

        Assert.Equal($"Product '{entityRequestDto.Name}' already exists.", exception.Message);

        _repositoryMock.Verify(repo => repo.GetByNameAsync(entityRequestDto.Name, cancellationToken), Times.Once);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenOperationIsCancelled_ShouldThrowTaskCanceledException()
    {
        // 1.ARRANGE
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _repositoryMock.Setup(repo => repo.GetAllAsync(cancellationToken))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));

        cts.Cancel();

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.GetAllAsync(cancellationToken));

        _repositoryMock.Verify(
            repo => repo.GetAllAsync(It.Is<CancellationToken>(ct => ct == cancellationToken)),
            Times.Once);
    }
}
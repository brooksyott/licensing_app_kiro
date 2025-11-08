using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceTests()
    {
        _loggerMock = new Mock<ILogger<ProductService>>();
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task CreateProductAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateProductAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.ProductCode, result.Data.ProductCode);
        Assert.Equal(request.Version, result.Data.Version);
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        var productId = Guid.NewGuid();
        
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetProductByIdAsync(productId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(productId, result.Data.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await service.GetProductByIdAsync(nonExistingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task UpdateProductAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        var productId = Guid.NewGuid();
        
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Original Name",
            ProductCode = "ORIG-001",
            Version = "1.0.0"
        });
        await context.SaveChangesAsync();

        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            ProductCode = "UPD-001",
            Version = "2.0.0",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateProductAsync(productId, updateRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Equal("UPD-001", result.Data.ProductCode);
        Assert.Equal("2.0.0", result.Data.Version);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        var productId = Guid.NewGuid();
        
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteProductAsync(productId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        
        var deletedProduct = await context.Products.FindAsync(productId);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task ListProductsAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        
        for (int i = 0; i < 5; i++)
        {
            context.Products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                ProductCode = $"PROD-{i:000}",
                Version = "1.0.0"
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListProductsAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(5, result.Data.Items.Count());
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchTerm_ReturnsMatchingProducts()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ProductService(context, _loggerMock.Object);
        
        context.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = "Alpha Product",
            ProductCode = "ALPHA-001",
            Version = "1.0.0"
        });
        context.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = "Beta Product",
            ProductCode = "BETA-001",
            Version = "1.0.0"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.SearchProductsAsync("Alpha", 1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Contains(result.Data, p => p.Name == "Alpha Product");
    }
}

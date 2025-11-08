using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class SkuServiceTests
{
    private readonly Mock<ILogger<SkuService>> _loggerMock;

    public SkuServiceTests()
    {
        _loggerMock = new Mock<ILogger<SkuService>>();
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task CreateSkuAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        await context.SaveChangesAsync();

        var request = new CreateSkuRequest
        {
            ProductId = productId,
            Name = "Test SKU",
            SkuCode = "SKU-001",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateSkuAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.SkuCode, result.Data.SkuCode);
        Assert.Equal(request.ProductId, result.Data.ProductId);
    }

    [Fact]
    public async Task CreateSkuAsync_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var request = new CreateSkuRequest
        {
            ProductId = Guid.NewGuid(),
            Name = "Test SKU",
            SkuCode = "SKU-001"
        };

        // Act
        var result = await service.CreateSkuAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task GetSkuByIdAsync_ExistingSku_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        
        var skuId = Guid.NewGuid();
        context.Skus.Add(new Sku
        {
            Id = skuId,
            ProductId = productId,
            Name = "Test SKU",
            SkuCode = "SKU-001"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetSkuByIdAsync(skuId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(skuId, result.Data.Id);
    }

    [Fact]
    public async Task GetSkuByIdAsync_NonExistingSku_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await service.GetSkuByIdAsync(nonExistingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task UpdateSkuAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        
        var skuId = Guid.NewGuid();
        context.Skus.Add(new Sku
        {
            Id = skuId,
            ProductId = productId,
            Name = "Original SKU",
            SkuCode = "ORIG-001"
        });
        await context.SaveChangesAsync();

        var updateRequest = new UpdateSkuRequest
        {
            ProductId = productId,
            Name = "Updated SKU",
            SkuCode = "UPD-001",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateSkuAsync(skuId, updateRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated SKU", result.Data.Name);
        Assert.Equal("UPD-001", result.Data.SkuCode);
    }

    [Fact]
    public async Task DeleteSkuAsync_ExistingSku_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        
        var skuId = Guid.NewGuid();
        context.Skus.Add(new Sku
        {
            Id = skuId,
            ProductId = productId,
            Name = "Test SKU",
            SkuCode = "SKU-001"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteSkuAsync(skuId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        
        var deletedSku = await context.Skus.FindAsync(skuId);
        Assert.Null(deletedSku);
    }

    [Fact]
    public async Task ListSkusAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        
        for (int i = 0; i < 5; i++)
        {
            context.Skus.Add(new Sku
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Name = $"SKU {i}",
                SkuCode = $"SKU-{i:000}"
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListSkusAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(5, result.Data.Items.Count());
    }

    [Fact]
    public async Task ListSkusByProductAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        
        context.Products.Add(new Product
        {
            Id = product1Id,
            Name = "Product 1",
            ProductCode = "PROD-001",
            Version = "1.0.0"
        });
        context.Products.Add(new Product
        {
            Id = product2Id,
            Name = "Product 2",
            ProductCode = "PROD-002",
            Version = "1.0.0"
        });
        
        context.Skus.Add(new Sku
        {
            Id = Guid.NewGuid(),
            ProductId = product1Id,
            Name = "SKU 1",
            SkuCode = "SKU-001"
        });
        context.Skus.Add(new Sku
        {
            Id = Guid.NewGuid(),
            ProductId = product1Id,
            Name = "SKU 2",
            SkuCode = "SKU-002"
        });
        context.Skus.Add(new Sku
        {
            Id = Guid.NewGuid(),
            ProductId = product2Id,
            Name = "SKU 3",
            SkuCode = "SKU-003"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListSkusByProductAsync(product1Id, 1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
        Assert.Equal(2, result.Data.Items.Count());
    }

    [Fact]
    public async Task SearchSkusAsync_WithSearchTerm_ReturnsMatchingSkus()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new SkuService(context, _loggerMock.Object);
        
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "TEST-001",
            Version = "1.0.0"
        });
        
        context.Skus.Add(new Sku
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = "Alpha SKU",
            SkuCode = "ALPHA-001"
        });
        context.Skus.Add(new Sku
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = "Beta SKU",
            SkuCode = "BETA-001"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.SearchSkusAsync("Alpha", 1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Contains(result.Data, s => s.Name == "Alpha SKU");
    }
}

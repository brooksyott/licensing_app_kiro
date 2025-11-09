using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;
using System.IdentityModel.Tokens.Jwt;

namespace LicenseManagementApi.Tests.Services;

/// <summary>
/// Integration tests for License Service with multiple SKUs per license functionality
/// </summary>
public class LicenseServiceMultipleSkusIntegrationTests
{
    private readonly Mock<ILogger<LicenseService>> _licenseServiceLoggerMock;
    private readonly Mock<ILogger<LicenseKeyGenerator>> _keyGeneratorLoggerMock;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public LicenseServiceMultipleSkusIntegrationTests()
    {
        _licenseServiceLoggerMock = new Mock<ILogger<LicenseService>>();
        _keyGeneratorLoggerMock = new Mock<ILogger<LicenseKeyGenerator>>();
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup configuration mock
        _configurationMock.Setup(c => c["RsaKey:Passphrase"]).Returns("test-passphrase");
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    private async Task<(Guid customerId, Guid productId, Guid rsaKeyId, List<Guid> skuIds)> SeedTestDataAsync(
        LicenseManagementDbContext context, 
        int skuCount = 2, 
        bool multipleProducts = false)
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        var skuIds = new List<Guid>();
        
        if (multipleProducts)
        {
            // Create SKUs from different products
            var product2Id = Guid.NewGuid();
            context.Products.Add(new Product
            {
                Id = product2Id,
                Name = "Test Product 2",
                ProductCode = "PROD-002",
                Version = "1.0"
            });

            for (int i = 0; i < skuCount; i++)
            {
                var skuId = Guid.NewGuid();
                var targetProductId = i % 2 == 0 ? productId : product2Id;
                
                context.Skus.Add(new Sku
                {
                    Id = skuId,
                    ProductId = targetProductId,
                    Name = $"Test SKU {i + 1}",
                    SkuCode = $"SKU-00{i + 1}",
                    Description = $"Test SKU Description {i + 1}"
                });
                
                skuIds.Add(skuId);
            }
        }
        else
        {
            // Create SKUs from same product
            for (int i = 0; i < skuCount; i++)
            {
                var skuId = Guid.NewGuid();
                
                context.Skus.Add(new Sku
                {
                    Id = skuId,
                    ProductId = productId,
                    Name = $"Test SKU {i + 1}",
                    SkuCode = $"SKU-00{i + 1}",
                    Description = $"Test SKU Description {i + 1}"
                });
                
                skuIds.Add(skuId);
            }
        }

        await context.SaveChangesAsync();

        return (customerId, productId, rsaKeyId, skuIds);
    }

    [Fact]
    public async Task CreateLicense_WithMultipleSkusFromSameProduct_CreatesAllAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 3, multipleProducts: false);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Enterprise",
            MaxActivations = 10
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Skus.Count);

        // Verify all SKU associations created in database
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == result.Data.Id)
            .ToListAsync();
        
        Assert.Equal(3, licenseSkus.Count);
        Assert.All(skuIds, skuId => Assert.Contains(licenseSkus, ls => ls.SkuId == skuId));
    }

    [Fact]
    public async Task CreateLicense_WithSkusFromDifferentProducts_CreatesAllAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 4, multipleProducts: true);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Enterprise",
            MaxActivations = 10
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(4, result.Data.Skus.Count);

        // Verify all SKU associations created in database
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == result.Data.Id)
            .Include(ls => ls.Sku)
                .ThenInclude(s => s.Product)
            .ToListAsync();
        
        Assert.Equal(4, licenseSkus.Count);
        
        // Verify we have SKUs from different products
        var distinctProducts = licenseSkus.Select(ls => ls.Sku.ProductId).Distinct().Count();
        Assert.Equal(2, distinctProducts);
    }

    [Fact]
    public async Task CreateLicense_WithDuplicateSkuIds_DeduplicatesAndSucceeds()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 2);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        // Add duplicate SKU IDs
        var requestSkuIds = new List<Guid> { skuIds[0], skuIds[1], skuIds[0], skuIds[1] };

        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = requestSkuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Skus.Count); // Should be deduplicated to 2

        // Verify only unique SKU associations created in database
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == result.Data.Id)
            .ToListAsync();
        
        Assert.Equal(2, licenseSkus.Count);
    }

    [Fact]
    public async Task CreateLicense_WithNoSkus_ReturnsValidationError()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, _) = await SeedTestDataAsync(context, skuCount: 0);

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = new List<Guid>(),
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        Assert.Contains("at least one SKU", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateLicense_WithInvalidSkuIds_ReturnsValidationError()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 1);

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var invalidSkuId = Guid.NewGuid();
        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = new List<Guid> { skuIds[0], invalidSkuId },
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        Assert.Contains("Invalid SKU IDs", result.ErrorMessage);
        Assert.Contains(invalidSkuId.ToString(), result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateLicense_AddingSkus_CreatesNewAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 3);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        // Create license with first SKU only
        var createRequest = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = new List<Guid> { skuIds[0] },
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        var createResult = await service.CreateLicenseAsync(createRequest);
        Assert.True(createResult.IsSuccess);

        // Act - Update to add more SKUs
        var updateRequest = new UpdateLicenseRequest
        {
            SkuIds = skuIds // All 3 SKUs
        };

        var updateResult = await service.UpdateLicenseAsync(createResult.Data!.Id, updateRequest);

        // Assert
        Assert.True(updateResult.IsSuccess);
        Assert.Equal(3, updateResult.Data!.Skus.Count);

        // Verify database has all 3 SKU associations
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == createResult.Data.Id)
            .ToListAsync();
        
        Assert.Equal(3, licenseSkus.Count);
    }

    [Fact]
    public async Task UpdateLicense_RemovingSkus_DeletesOldAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 3);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        // Create license with all 3 SKUs
        var createRequest = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        var createResult = await service.CreateLicenseAsync(createRequest);
        Assert.True(createResult.IsSuccess);

        // Act - Update to keep only first SKU
        var updateRequest = new UpdateLicenseRequest
        {
            SkuIds = new List<Guid> { skuIds[0] }
        };

        var updateResult = await service.UpdateLicenseAsync(createResult.Data!.Id, updateRequest);

        // Assert
        Assert.True(updateResult.IsSuccess);
        Assert.Single(updateResult.Data!.Skus);

        // Verify database has only 1 SKU association
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == createResult.Data.Id)
            .ToListAsync();
        
        Assert.Single(licenseSkus);
        Assert.Equal(skuIds[0], licenseSkus[0].SkuId);
    }

    [Fact]
    public async Task UpdateLicense_ReplacingAllSkus_UpdatesAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 4);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        // Create license with first 2 SKUs
        var createRequest = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds.Take(2).ToList(),
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        var createResult = await service.CreateLicenseAsync(createRequest);
        Assert.True(createResult.IsSuccess);

        // Act - Update to use last 2 SKUs (completely different set)
        var updateRequest = new UpdateLicenseRequest
        {
            SkuIds = skuIds.Skip(2).Take(2).ToList()
        };

        var updateResult = await service.UpdateLicenseAsync(createResult.Data!.Id, updateRequest);

        // Assert
        Assert.True(updateResult.IsSuccess);
        Assert.Equal(2, updateResult.Data!.Skus.Count);

        // Verify database has the new SKU associations
        var licenseSkus = await context.LicenseSkus
            .Where(ls => ls.LicenseId == createResult.Data.Id)
            .ToListAsync();
        
        Assert.Equal(2, licenseSkus.Count);
        Assert.Contains(licenseSkus, ls => ls.SkuId == skuIds[2]);
        Assert.Contains(licenseSkus, ls => ls.SkuId == skuIds[3]);
        Assert.DoesNotContain(licenseSkus, ls => ls.SkuId == skuIds[0]);
        Assert.DoesNotContain(licenseSkus, ls => ls.SkuId == skuIds[1]);
    }

    [Fact]
    public async Task GetLicenseById_WithMultipleSkus_ReturnsAllSkuData()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 3);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var createRequest = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Enterprise",
            MaxActivations = 10
        };

        var createResult = await service.CreateLicenseAsync(createRequest);
        Assert.True(createResult.IsSuccess);

        // Act
        var getResult = await service.GetLicenseByIdAsync(createResult.Data!.Id);

        // Assert
        Assert.True(getResult.IsSuccess);
        Assert.NotNull(getResult.Data);
        Assert.Equal(3, getResult.Data.Skus.Count);
        
        // Verify SKU details are populated
        foreach (var sku in getResult.Data.Skus)
        {
            Assert.NotEqual(Guid.Empty, sku.SkuId);
            Assert.NotEmpty(sku.SkuName);
            Assert.NotEmpty(sku.SkuCode);
        }
    }

    [Fact]
    public async Task ListLicenses_WithMultipleSkus_ReturnsAllSkuData()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 2);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        // Create two licenses with different SKU combinations
        var createRequest1 = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = new List<Guid> { skuIds[0] },
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        var createRequest2 = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Enterprise",
            MaxActivations = 10
        };

        await service.CreateLicenseAsync(createRequest1);
        await service.CreateLicenseAsync(createRequest2);

        // Act
        var listResult = await service.ListLicensesAsync(1, 10);

        // Assert
        Assert.True(listResult.IsSuccess);
        Assert.NotNull(listResult.Data);
        Assert.Equal(2, listResult.Data.Items.Count());
        
        var license1 = listResult.Data.Items.First(l => l.LicenseType == "Standard");
        var license2 = listResult.Data.Items.First(l => l.LicenseType == "Enterprise");
        
        Assert.Single(license1.Skus);
        Assert.Equal(2, license2.Skus.Count);
    }

    [Fact]
    public async Task DeleteLicense_WithMultipleSkus_RemovesAllSkuAssociations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var (customerId, productId, rsaKeyId, skuIds) = await SeedTestDataAsync(context, skuCount: 3);

        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var keyGenerator = new LicenseKeyGenerator(
            context,
            _cryptographyServiceMock.Object,
            _configurationMock.Object,
            _keyGeneratorLoggerMock.Object);

        var service = new LicenseService(
            context,
            _licenseServiceLoggerMock.Object,
            keyGenerator,
            _cryptographyServiceMock.Object);

        var createRequest = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            SkuIds = skuIds,
            RsaKeyId = rsaKeyId,
            LicenseType = "Enterprise",
            MaxActivations = 10
        };

        var createResult = await service.CreateLicenseAsync(createRequest);
        Assert.True(createResult.IsSuccess);

        var licenseId = createResult.Data!.Id;

        // Verify SKU associations exist before deletion
        var skuAssociationsBeforeDelete = await context.LicenseSkus
            .Where(ls => ls.LicenseId == licenseId)
            .ToListAsync();
        Assert.Equal(3, skuAssociationsBeforeDelete.Count);

        // Act
        var deleteResult = await service.DeleteLicenseAsync(licenseId);

        // Assert
        Assert.True(deleteResult.IsSuccess);

        // Verify license is deleted
        var deletedLicense = await context.Licenses.FindAsync(licenseId);
        Assert.Null(deletedLicense);

        // Verify all SKU associations are removed
        var skuAssociationsAfterDelete = await context.LicenseSkus
            .Where(ls => ls.LicenseId == licenseId)
            .ToListAsync();
        Assert.Empty(skuAssociationsAfterDelete);

        // Verify SKUs themselves still exist
        var skusStillExist = await context.Skus
            .Where(s => skuIds.Contains(s.Id))
            .ToListAsync();
        Assert.Equal(3, skusStillExist.Count);
    }
}

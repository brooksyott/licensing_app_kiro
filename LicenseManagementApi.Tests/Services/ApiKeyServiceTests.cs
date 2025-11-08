using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class ApiKeyServiceTests
{
    private readonly Mock<ILogger<ApiKeyService>> _loggerMock;
    private readonly IMemoryCache _cache;

    public ApiKeyServiceTests()
    {
        _loggerMock = new Mock<ILogger<ApiKeyService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task CreateApiKeyAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var request = new CreateApiKeyRequest
        {
            Name = "Test API Key",
            Role = "Admin",
            CreatedBy = "test@example.com",
            IsActive = true
        };

        // Act
        var result = await service.CreateApiKeyAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.Role, result.Data.Role);
        Assert.NotNull(result.Data.ApiKey); // Plain API key should be returned on creation
        Assert.NotEmpty(result.Data.ApiKey);
    }

    [Fact]
    public async Task CreateApiKeyAsync_GeneratesUniqueKey()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var request1 = new CreateApiKeyRequest
        {
            Name = "API Key 1",
            Role = "Admin",
            CreatedBy = "test@example.com"
        };
        var request2 = new CreateApiKeyRequest
        {
            Name = "API Key 2",
            Role = "User",
            CreatedBy = "test@example.com"
        };

        // Act
        var result1 = await service.CreateApiKeyAsync(request1);
        var result2 = await service.CreateApiKeyAsync(request2);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.NotEqual(result1.Data.ApiKey, result2.Data.ApiKey);
    }

    [Fact]
    public async Task GetApiKeyByIdAsync_ExistingKey_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var apiKeyId = Guid.NewGuid();
        
        context.ApiKeys.Add(new ApiKey
        {
            Id = apiKeyId,
            Name = "Test API Key",
            KeyHash = "test-hash",
            Role = "Admin",
            IsActive = true,
            CreatedBy = "test@example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetApiKeyByIdAsync(apiKeyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(apiKeyId, result.Data.Id);
        Assert.Equal("Test API Key", result.Data.Name);
        Assert.Null(result.Data.ApiKey); // Plain key should not be returned on retrieval
    }

    [Fact]
    public async Task GetApiKeyByIdAsync_NonExistingKey_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await service.GetApiKeyByIdAsync(nonExistingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task UpdateApiKeyAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var apiKeyId = Guid.NewGuid();
        
        context.ApiKeys.Add(new ApiKey
        {
            Id = apiKeyId,
            Name = "Original Name",
            KeyHash = "test-hash",
            Role = "User",
            IsActive = true,
            CreatedBy = "test@example.com"
        });
        await context.SaveChangesAsync();

        var updateRequest = new UpdateApiKeyRequest
        {
            Name = "Updated Name",
            Role = "Admin",
            IsActive = false
        };

        // Act
        var result = await service.UpdateApiKeyAsync(apiKeyId, updateRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Equal("Admin", result.Data.Role);
        Assert.False(result.Data.IsActive);
    }

    [Fact]
    public async Task DeleteApiKeyAsync_ExistingKey_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        var apiKeyId = Guid.NewGuid();
        
        context.ApiKeys.Add(new ApiKey
        {
            Id = apiKeyId,
            Name = "Test API Key",
            KeyHash = "test-hash",
            Role = "Admin",
            IsActive = true,
            CreatedBy = "test@example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteApiKeyAsync(apiKeyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        
        var deletedKey = await context.ApiKeys.FindAsync(apiKeyId);
        Assert.Null(deletedKey);
    }

    [Fact]
    public async Task ListApiKeysAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        
        for (int i = 0; i < 5; i++)
        {
            context.ApiKeys.Add(new ApiKey
            {
                Id = Guid.NewGuid(),
                Name = $"API Key {i}",
                KeyHash = $"hash-{i}",
                Role = "User",
                IsActive = true,
                CreatedBy = "test@example.com"
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListApiKeysAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(5, result.Data.Items.Count());
    }

    [Fact]
    public async Task ValidateApiKeyAsync_ValidKey_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        
        // Create an API key
        var createRequest = new CreateApiKeyRequest
        {
            Name = "Test API Key",
            Role = "Admin",
            CreatedBy = "test@example.com",
            IsActive = true
        };
        var createResult = await service.CreateApiKeyAsync(createRequest);
        var plainApiKey = createResult.Data.ApiKey;

        // Act
        var validateResult = await service.ValidateApiKeyAsync(plainApiKey!);

        // Assert
        Assert.True(validateResult.IsSuccess);
        Assert.NotNull(validateResult.Data);
        Assert.True(validateResult.Data.IsValid);
        Assert.Equal("Admin", validateResult.Data.Role);
        Assert.Equal("Test API Key", validateResult.Data.Name);
    }

    [Fact]
    public async Task ValidateApiKeyAsync_InvalidKey_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);

        // Act
        var result = await service.ValidateApiKeyAsync("invalid-key");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsValid);
    }

    [Fact]
    public async Task ValidateApiKeyAsync_InactiveKey_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        
        // Create an API key
        var createRequest = new CreateApiKeyRequest
        {
            Name = "Test API Key",
            Role = "Admin",
            CreatedBy = "test@example.com",
            IsActive = true
        };
        var createResult = await service.CreateApiKeyAsync(createRequest);
        var plainApiKey = createResult.Data.ApiKey;
        var apiKeyId = createResult.Data.Id;

        // Deactivate the key
        var updateRequest = new UpdateApiKeyRequest
        {
            Name = "Test API Key",
            Role = "Admin",
            IsActive = false
        };
        await service.UpdateApiKeyAsync(apiKeyId, updateRequest);

        // Act
        var validateResult = await service.ValidateApiKeyAsync(plainApiKey!);

        // Assert
        Assert.True(validateResult.IsSuccess);
        Assert.NotNull(validateResult.Data);
        Assert.False(validateResult.Data.IsValid);
    }

    [Fact]
    public async Task ValidateApiKeyAsync_UpdatesLastUsedTimestamp()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ApiKeyService(context, _loggerMock.Object, _cache);
        
        // Create an API key
        var createRequest = new CreateApiKeyRequest
        {
            Name = "Test API Key",
            Role = "Admin",
            CreatedBy = "test@example.com",
            IsActive = true
        };
        var createResult = await service.CreateApiKeyAsync(createRequest);
        var plainApiKey = createResult.Data.ApiKey;
        var apiKeyId = createResult.Data.Id;

        // Act
        await service.ValidateApiKeyAsync(plainApiKey!);

        // Assert
        var apiKey = await context.ApiKeys.FindAsync(apiKeyId);
        Assert.NotNull(apiKey);
        Assert.NotNull(apiKey.LastUsedAt);
    }
}

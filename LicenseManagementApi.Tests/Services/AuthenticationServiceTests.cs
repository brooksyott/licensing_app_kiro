using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IApiKeyService> _apiKeyServiceMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly IMemoryCache _cache;

    public AuthenticationServiceTests()
    {
        _apiKeyServiceMock = new Mock<IApiKeyService>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task AuthenticateAsync_ValidApiKey_ReturnsAuthenticated()
    {
        // Arrange
        var apiKey = "valid-api-key";
        var validationResult = new ApiKeyValidationResult
        {
            IsValid = true,
            ApiKeyId = Guid.NewGuid(),
            Role = "Admin",
            Name = "Test API Key"
        };

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Success(validationResult));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result = await service.AuthenticateAsync(apiKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.IsAuthenticated);
        Assert.Equal(validationResult.ApiKeyId, result.Data.ApiKeyId);
        Assert.Equal(validationResult.Role, result.Data.Role);
        Assert.Equal(validationResult.Name, result.Data.Name);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidApiKey_ReturnsNotAuthenticated()
    {
        // Arrange
        var apiKey = "invalid-api-key";
        var validationResult = new ApiKeyValidationResult
        {
            IsValid = false
        };

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Success(validationResult));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result = await service.AuthenticateAsync(apiKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsAuthenticated);
    }

    [Fact]
    public async Task AuthenticateAsync_EmptyApiKey_ReturnsNotAuthenticated()
    {
        // Arrange
        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result = await service.AuthenticateAsync("");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsAuthenticated);
        _apiKeyServiceMock.Verify(x => x.ValidateApiKeyAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_CachesValidResult()
    {
        // Arrange
        var apiKey = "valid-api-key";
        var validationResult = new ApiKeyValidationResult
        {
            IsValid = true,
            ApiKeyId = Guid.NewGuid(),
            Role = "Admin",
            Name = "Test API Key"
        };

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Success(validationResult));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result1 = await service.AuthenticateAsync(apiKey);
        var result2 = await service.AuthenticateAsync(apiKey);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result1.Data.IsAuthenticated);
        Assert.True(result2.Data.IsAuthenticated);
        
        // Verify that ValidateApiKeyAsync was only called once (second call used cache)
        _apiKeyServiceMock.Verify(x => x.ValidateApiKeyAsync(apiKey), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_DoesNotCacheInvalidResult()
    {
        // Arrange
        var apiKey = "invalid-api-key";
        var validationResult = new ApiKeyValidationResult
        {
            IsValid = false
        };

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Success(validationResult));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result1 = await service.AuthenticateAsync(apiKey);
        var result2 = await service.AuthenticateAsync(apiKey);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.False(result1.Data.IsAuthenticated);
        Assert.False(result2.Data.IsAuthenticated);
        
        // Verify that ValidateApiKeyAsync was called twice (no caching for invalid keys)
        _apiKeyServiceMock.Verify(x => x.ValidateApiKeyAsync(apiKey), Times.Exactly(2));
    }

    [Fact]
    public async Task AuthenticateAsync_ValidationServiceError_ReturnsFailure()
    {
        // Arrange
        var apiKey = "test-api-key";

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Failure("Service error", "INTERNAL_ERROR"));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        var result = await service.AuthenticateAsync(apiKey);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Service error", result.ErrorMessage);
        Assert.Equal("INTERNAL_ERROR", result.ErrorCode);
    }

    [Fact]
    public async Task InvalidateCacheAsync_RemovesCachedEntry()
    {
        // Arrange
        var apiKey = "valid-api-key";
        var validationResult = new ApiKeyValidationResult
        {
            IsValid = true,
            ApiKeyId = Guid.NewGuid(),
            Role = "Admin",
            Name = "Test API Key"
        };

        _apiKeyServiceMock
            .Setup(x => x.ValidateApiKeyAsync(apiKey))
            .ReturnsAsync(ServiceResult<ApiKeyValidationResult>.Success(validationResult));

        var service = new AuthenticationService(_apiKeyServiceMock.Object, _cache, _loggerMock.Object);

        // Act
        // First authenticate to cache the result
        await service.AuthenticateAsync(apiKey);
        
        // Invalidate the cache
        await service.InvalidateCacheAsync(apiKey);
        
        // Authenticate again
        await service.AuthenticateAsync(apiKey);

        // Assert
        // Verify that ValidateApiKeyAsync was called twice (once before invalidation, once after)
        _apiKeyServiceMock.Verify(x => x.ValidateApiKeyAsync(apiKey), Times.Exactly(2));
    }
}

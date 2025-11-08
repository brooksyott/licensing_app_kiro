using Microsoft.Extensions.Caching.Memory;
using LicenseManagementApi.Models;

namespace LicenseManagementApi.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuthenticationService> _logger;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public AuthenticationService(
        IApiKeyService apiKeyService,
        IMemoryCache cache,
        ILogger<AuthenticationService> logger)
    {
        _apiKeyService = apiKeyService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ServiceResult<AuthenticationResult>> AuthenticateAsync(string apiKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult
                {
                    IsAuthenticated = false
                });
            }

            // Generate cache key from API key hash
            var cacheKey = $"auth_{apiKey}";

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out AuthenticationResult? cachedResult) && cachedResult != null)
            {
                _logger.LogDebug("Authentication result retrieved from cache");
                return ServiceResult<AuthenticationResult>.Success(cachedResult);
            }

            // Validate API key
            var validationResult = await _apiKeyService.ValidateApiKeyAsync(apiKey);

            if (!validationResult.IsSuccess)
            {
                _logger.LogError("Error validating API key: {ErrorMessage}", validationResult.ErrorMessage);
                return ServiceResult<AuthenticationResult>.Failure(
                    validationResult.ErrorMessage,
                    validationResult.ErrorCode);
            }

            var authResult = new AuthenticationResult
            {
                IsAuthenticated = validationResult.Data.IsValid,
                ApiKeyId = validationResult.Data.ApiKeyId,
                Role = validationResult.Data.Role,
                Name = validationResult.Data.Name
            };

            // Cache the result if authenticated
            if (authResult.IsAuthenticated)
            {
                _cache.Set(cacheKey, authResult, CacheDuration);
                _logger.LogDebug("Authentication result cached for 5 minutes");
            }

            return ServiceResult<AuthenticationResult>.Success(authResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return ServiceResult<AuthenticationResult>.Failure(
                "An error occurred during authentication",
                "INTERNAL_ERROR");
        }
    }

    public Task InvalidateCacheAsync(string apiKey)
    {
        try
        {
            var cacheKey = $"auth_{apiKey}";
            _cache.Remove(cacheKey);
            _logger.LogInformation("Cache invalidated for API key");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache");
            return Task.CompletedTask;
        }
    }
}

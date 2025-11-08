using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using System.Security.Cryptography;
using System.Text;

namespace LicenseManagementApi.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<ApiKeyService> _logger;
    private readonly IMemoryCache _cache;
    private const int ApiKeyLength = 32;
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public ApiKeyService(LicenseManagementDbContext context, ILogger<ApiKeyService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ServiceResult<ApiKeyDto>> CreateApiKeyAsync(CreateApiKeyRequest request)
    {
        try
        {
            // Generate unique API key
            string apiKey;
            string apiKeyHash;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                apiKey = GenerateApiKey();
                apiKeyHash = HashApiKey(apiKey);
                
                var existingKey = await _context.ApiKeys
                    .FirstOrDefaultAsync(k => k.KeyHash == apiKeyHash);
                
                if (existingKey == null)
                    break;

                attempts++;
            } while (attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                return ServiceResult<ApiKeyDto>.Failure(
                    "Failed to generate unique API key",
                    "INTERNAL_ERROR");
            }

            var apiKeyEntity = new ApiKey
            {
                Id = Guid.NewGuid(),
                KeyHash = apiKeyHash,
                Name = request.Name,
                Role = request.Role,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy
            };

            _context.ApiKeys.Add(apiKeyEntity);
            await _context.SaveChangesAsync();

            // Store mapping of API key ID to cache key for later invalidation
            var cacheKeyMapping = $"apikey_mapping_{apiKeyEntity.Id}";
            var authCacheKey = $"auth_{apiKey}";
            _cache.Set(cacheKeyMapping, authCacheKey, TimeSpan.FromDays(365));

            _logger.LogInformation("API key created with ID: {ApiKeyId}, Name: {Name}", 
                apiKeyEntity.Id, apiKeyEntity.Name);

            var dto = MapToDto(apiKeyEntity);
            dto.ApiKey = apiKey; // Only return the plain API key on creation

            return ServiceResult<ApiKeyDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating API key");
            return ServiceResult<ApiKeyDto>.Failure(
                "An error occurred while creating the API key",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<ApiKeyDto>> GetApiKeyByIdAsync(Guid id)
    {
        try
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);

            if (apiKey == null)
            {
                return ServiceResult<ApiKeyDto>.Failure(
                    $"API key with ID {id} not found",
                    "NOT_FOUND");
            }

            return ServiceResult<ApiKeyDto>.Success(MapToDto(apiKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving API key with ID: {ApiKeyId}", id);
            return ServiceResult<ApiKeyDto>.Failure(
                "An error occurred while retrieving the API key",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<ApiKeyDto>> UpdateApiKeyAsync(Guid id, UpdateApiKeyRequest request)
    {
        try
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);

            if (apiKey == null)
            {
                return ServiceResult<ApiKeyDto>.Failure(
                    $"API key with ID {id} not found",
                    "NOT_FOUND");
            }

            apiKey.Name = request.Name;
            apiKey.Role = request.Role;
            apiKey.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            _logger.LogInformation("API key updated with ID: {ApiKeyId}", id);

            return ServiceResult<ApiKeyDto>.Success(MapToDto(apiKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating API key with ID: {ApiKeyId}", id);
            return ServiceResult<ApiKeyDto>.Failure(
                "An error occurred while updating the API key",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteApiKeyAsync(Guid id)
    {
        try
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);

            if (apiKey == null)
            {
                return ServiceResult<bool>.Failure(
                    $"API key with ID {id} not found",
                    "NOT_FOUND");
            }

            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            // Invalidate cache entry for this API key
            var cacheKeyMapping = $"apikey_mapping_{id}";
            if (_cache.TryGetValue(cacheKeyMapping, out string? authCacheKey) && authCacheKey != null)
            {
                _cache.Remove(authCacheKey);
                _cache.Remove(cacheKeyMapping);
                _logger.LogInformation("API key deleted with ID: {ApiKeyId} and cache invalidated", id);
            }
            else
            {
                _logger.LogInformation("API key deleted with ID: {ApiKeyId}", id);
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting API key with ID: {ApiKeyId}", id);
            return ServiceResult<bool>.Failure(
                "An error occurred while deleting the API key",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<ApiKeyDto>>> ListApiKeysAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.ApiKeys.CountAsync();

            var apiKeys = await _context.ApiKeys
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var apiKeyDtos = apiKeys.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<ApiKeyDto>(apiKeyDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<ApiKeyDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing API keys");
            return ServiceResult<PagedResult<ApiKeyDto>>.Failure(
                "An error occurred while listing API keys",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<ApiKeyValidationResult>> ValidateApiKeyAsync(string apiKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ServiceResult<ApiKeyValidationResult>.Success(new ApiKeyValidationResult
                {
                    IsValid = false
                });
            }

            // Find API key by comparing hashes
            var apiKeys = await _context.ApiKeys
                .Where(k => k.IsActive)
                .ToListAsync();

            ApiKey? matchedKey = null;
            foreach (var key in apiKeys)
            {
                if (VerifyApiKey(apiKey, key.KeyHash))
                {
                    matchedKey = key;
                    break;
                }
            }

            if (matchedKey == null)
            {
                return ServiceResult<ApiKeyValidationResult>.Success(new ApiKeyValidationResult
                {
                    IsValid = false
                });
            }

            // Update last used timestamp
            matchedKey.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ServiceResult<ApiKeyValidationResult>.Success(new ApiKeyValidationResult
            {
                IsValid = true,
                ApiKeyId = matchedKey.Id,
                Role = matchedKey.Role,
                Name = matchedKey.Name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return ServiceResult<ApiKeyValidationResult>.Failure(
                "An error occurred while validating the API key",
                "INTERNAL_ERROR");
        }
    }

    private string GenerateApiKey()
    {
        var keyBytes = new byte[ApiKeyLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }

        var result = new StringBuilder(ApiKeyLength);
        foreach (var b in keyBytes)
        {
            result.Append(AllowedCharacters[b % AllowedCharacters.Length]);
        }

        return result.ToString();
    }

    private string HashApiKey(string apiKey)
    {
        // Use PBKDF2 with SHA256 for secure hashing with salt
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(
            apiKey,
            salt,
            100000, // iterations
            HashAlgorithmName.SHA256
        );

        var hash = pbkdf2.GetBytes(32);
        
        // Combine salt and hash for storage
        var hashBytes = new byte[48]; // 16 bytes salt + 32 bytes hash
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyApiKey(string apiKey, string hash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            
            if (hashBytes.Length != 48)
                return false;

            // Extract salt
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Extract stored hash
            var storedHash = new byte[32];
            Array.Copy(hashBytes, 16, storedHash, 0, 32);

            // Compute hash of provided key
            using var pbkdf2 = new Rfc2898DeriveBytes(
                apiKey,
                salt,
                100000,
                HashAlgorithmName.SHA256
            );

            var computedHash = pbkdf2.GetBytes(32);

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    private static ApiKeyDto MapToDto(ApiKey apiKey)
    {
        return new ApiKeyDto
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            Role = apiKey.Role,
            IsActive = apiKey.IsActive,
            CreatedBy = apiKey.CreatedBy,
            CreatedAt = apiKey.CreatedAt,
            UpdatedAt = apiKey.UpdatedAt,
            LastUsedAt = apiKey.LastUsedAt
            // ApiKey is not included here - only returned on creation
        };
    }
}

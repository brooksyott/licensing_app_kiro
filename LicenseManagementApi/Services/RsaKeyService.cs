using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public class RsaKeyService : IRsaKeyService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ICryptographyService _cryptographyService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RsaKeyService> _logger;

    public RsaKeyService(
        LicenseManagementDbContext context,
        ICryptographyService cryptographyService,
        IConfiguration configuration,
        ILogger<RsaKeyService> logger)
    {
        _context = context;
        _cryptographyService = cryptographyService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ServiceResult<RsaKeyDto>> GenerateKeyPairAsync(GenerateRsaKeyRequest request)
    {
        try
        {
            // Generate RSA key pair
            var (publicKey, privateKey) = _cryptographyService.GenerateRsaKeyPair(request.KeySize);

            // Get encryption passphrase from configuration
            var passphrase = _configuration["Cryptography:PrivateKeyPassphrase"] 
                ?? throw new InvalidOperationException("Private key passphrase not configured");

            // Encrypt the private key
            var encryptedPrivateKey = _cryptographyService.EncryptPrivateKey(privateKey, passphrase);

            var rsaKey = new RsaKey
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PublicKey = publicKey,
                PrivateKeyEncrypted = encryptedPrivateKey,
                KeySize = request.KeySize,
                CreatedBy = request.CreatedBy
            };

            _context.RsaKeys.Add(rsaKey);
            await _context.SaveChangesAsync();

            _logger.LogInformation("RSA key pair generated with ID: {RsaKeyId}", rsaKey.Id);

            return ServiceResult<RsaKeyDto>.Success(MapToDto(rsaKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating RSA key pair");
            return ServiceResult<RsaKeyDto>.Failure("An error occurred while generating the RSA key pair", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<RsaKeyDto>> GetKeyByIdAsync(Guid id)
    {
        try
        {
            var rsaKey = await _context.RsaKeys.FindAsync(id);

            if (rsaKey == null)
            {
                return ServiceResult<RsaKeyDto>.Failure($"RSA key with ID {id} not found", "NOT_FOUND");
            }

            return ServiceResult<RsaKeyDto>.Success(MapToDto(rsaKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving RSA key with ID: {RsaKeyId}", id);
            return ServiceResult<RsaKeyDto>.Failure("An error occurred while retrieving the RSA key", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<string>> DownloadPrivateKeyAsync(Guid id)
    {
        try
        {
            var rsaKey = await _context.RsaKeys.FindAsync(id);

            if (rsaKey == null)
            {
                return ServiceResult<string>.Failure($"RSA key with ID {id} not found", "NOT_FOUND");
            }

            // Get decryption passphrase from configuration
            var passphrase = _configuration["Cryptography:PrivateKeyPassphrase"]
                ?? throw new InvalidOperationException("Private key passphrase not configured");

            // Decrypt the private key
            var privateKey = _cryptographyService.DecryptPrivateKey(rsaKey.PrivateKeyEncrypted, passphrase);

            _logger.LogInformation("Private key downloaded for RSA key ID: {RsaKeyId}", id);

            return ServiceResult<string>.Success(privateKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading private key for RSA key ID: {RsaKeyId}", id);
            return ServiceResult<string>.Failure("An error occurred while downloading the private key", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<RsaKeyDto>> UpdateKeyAsync(Guid id, UpdateRsaKeyRequest request)
    {
        try
        {
            var rsaKey = await _context.RsaKeys.FindAsync(id);

            if (rsaKey == null)
            {
                return ServiceResult<RsaKeyDto>.Failure($"RSA key with ID {id} not found", "NOT_FOUND");
            }

            rsaKey.Name = request.Name;

            await _context.SaveChangesAsync();

            _logger.LogInformation("RSA key updated with ID: {RsaKeyId}", rsaKey.Id);

            return ServiceResult<RsaKeyDto>.Success(MapToDto(rsaKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating RSA key with ID: {RsaKeyId}", id);
            return ServiceResult<RsaKeyDto>.Failure("An error occurred while updating the RSA key", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteKeyAsync(Guid id)
    {
        try
        {
            var rsaKey = await _context.RsaKeys.FindAsync(id);

            if (rsaKey == null)
            {
                return ServiceResult<bool>.Failure($"RSA key with ID {id} not found", "NOT_FOUND");
            }

            _context.RsaKeys.Remove(rsaKey);
            await _context.SaveChangesAsync();

            _logger.LogInformation("RSA key deleted with ID: {RsaKeyId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting RSA key with ID: {RsaKeyId}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the RSA key", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<RsaKeyDto>>> ListKeysAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.RsaKeys.CountAsync();

            var rsaKeys = await _context.RsaKeys
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var rsaKeyDtos = rsaKeys.Select(MapToDto);

            var pagedResult = new PagedResult<RsaKeyDto>(rsaKeyDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<RsaKeyDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing RSA keys");
            return ServiceResult<PagedResult<RsaKeyDto>>.Failure("An error occurred while listing RSA keys", "INTERNAL_ERROR");
        }
    }

    private static RsaKeyDto MapToDto(RsaKey rsaKey)
    {
        return new RsaKeyDto
        {
            Id = rsaKey.Id,
            Name = rsaKey.Name,
            PublicKey = rsaKey.PublicKey,
            KeySize = rsaKey.KeySize,
            CreatedBy = rsaKey.CreatedBy,
            CreatedAt = rsaKey.CreatedAt,
            UpdatedAt = rsaKey.UpdatedAt
        };
    }
}

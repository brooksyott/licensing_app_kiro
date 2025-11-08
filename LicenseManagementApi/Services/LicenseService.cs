using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using System.Text.Json;

namespace LicenseManagementApi.Services;

public class LicenseService : ILicenseService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<LicenseService> _logger;
    private readonly ILicenseKeyGenerator _keyGenerator;
    private readonly ICryptographyService _cryptographyService;

    public LicenseService(
        LicenseManagementDbContext context,
        ILogger<LicenseService> logger,
        ILicenseKeyGenerator keyGenerator,
        ICryptographyService cryptographyService)
    {
        _context = context;
        _logger = logger;
        _keyGenerator = keyGenerator;
        _cryptographyService = cryptographyService;
    }

    public async Task<ServiceResult<LicenseDto>> CreateLicenseAsync(CreateLicenseRequest request)
    {
        try
        {
            // Validate customer exists
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return ServiceResult<LicenseDto>.Failure(
                    $"Customer with ID {request.CustomerId} not found",
                    "NOT_FOUND");
            }

            // Validate product exists
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return ServiceResult<LicenseDto>.Failure(
                    $"Product with ID {request.ProductId} not found",
                    "NOT_FOUND");
            }

            // Validate SKU if provided
            if (request.SkuId.HasValue)
            {
                var sku = await _context.Skus.FindAsync(request.SkuId.Value);
                if (sku == null)
                {
                    return ServiceResult<LicenseDto>.Failure(
                        $"SKU with ID {request.SkuId.Value} not found",
                        "NOT_FOUND");
                }

                // Ensure SKU belongs to the product
                if (sku.ProductId != request.ProductId)
                {
                    return ServiceResult<LicenseDto>.Failure(
                        "SKU does not belong to the specified product",
                        "VALIDATION_ERROR");
                }
            }

            // Validate RSA key exists
            var rsaKey = await _context.RsaKeys.FindAsync(request.RsaKeyId);
            if (rsaKey == null)
            {
                return ServiceResult<LicenseDto>.Failure(
                    $"RSA key with ID {request.RsaKeyId} not found",
                    "NOT_FOUND");
            }

            // Generate unique license key
            string licenseKey;
            string licenseKeyHash;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                licenseKey = _keyGenerator.GenerateLicenseKey();
                licenseKeyHash = _keyGenerator.HashLicenseKey(licenseKey);
                
                var existingLicense = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.LicenseKeyHash == licenseKeyHash);
                
                if (existingLicense == null)
                    break;

                attempts++;
            } while (attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                return ServiceResult<LicenseDto>.Failure(
                    "Failed to generate unique license key",
                    "INTERNAL_ERROR");
            }

            // Create license payload for signing
            var payload = new
            {
                LicenseKey = licenseKey,
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                SkuId = request.SkuId,
                LicenseType = request.LicenseType,
                ExpirationDate = request.ExpirationDate,
                MaxActivations = request.MaxActivations,
                IssuedAt = DateTime.UtcNow
            };

            var payloadJson = JsonSerializer.Serialize(payload);

            // Decrypt private key and sign the payload
            string privateKey;
            try
            {
                // Note: In production, the passphrase should come from secure configuration
                privateKey = _cryptographyService.DecryptPrivateKey(
                    rsaKey.PrivateKeyEncrypted,
                    Environment.GetEnvironmentVariable("RSA_KEY_PASSPHRASE") ?? "default-passphrase");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt private key for RSA key ID: {RsaKeyId}", request.RsaKeyId);
                return ServiceResult<LicenseDto>.Failure(
                    "Failed to decrypt RSA private key",
                    "INTERNAL_ERROR");
            }

            string signedPayload;
            try
            {
                signedPayload = _cryptographyService.SignData(payloadJson, privateKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sign license payload");
                return ServiceResult<LicenseDto>.Failure(
                    "Failed to sign license",
                    "INTERNAL_ERROR");
            }

            // Create license entity
            var license = new License
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                SkuId = request.SkuId,
                RsaKeyId = request.RsaKeyId,
                LicenseKey = licenseKey,
                LicenseKeyHash = licenseKeyHash,
                SignedPayload = signedPayload,
                LicenseType = request.LicenseType,
                ExpirationDate = request.ExpirationDate,
                MaxActivations = request.MaxActivations,
                CurrentActivations = 0,
                Status = LicenseStatus.Active
            };

            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();

            _logger.LogInformation("License created with ID: {LicenseId} for Customer: {CustomerId}", 
                license.Id, request.CustomerId);

            // Load navigation properties for DTO mapping
            await _context.Entry(license).Reference(l => l.Customer).LoadAsync();
            await _context.Entry(license).Reference(l => l.Product).LoadAsync();
            if (license.SkuId.HasValue)
            {
                await _context.Entry(license).Reference(l => l.Sku).LoadAsync();
            }
            await _context.Entry(license).Reference(l => l.RsaKey).LoadAsync();

            return ServiceResult<LicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating license");
            return ServiceResult<LicenseDto>.Failure(
                "An error occurred while creating the license",
                "INTERNAL_ERROR");
        }
    }

    private LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            CustomerId = license.CustomerId,
            CustomerName = license.Customer?.Name ?? string.Empty,
            ProductId = license.ProductId,
            ProductName = license.Product?.Name ?? string.Empty,
            SkuId = license.SkuId,
            SkuName = license.Sku?.Name,
            RsaKeyId = license.RsaKeyId,
            RsaKeyName = license.RsaKey?.Name ?? string.Empty,
            LicenseKey = license.LicenseKey,
            SignedPayload = license.SignedPayload,
            LicenseType = license.LicenseType,
            ExpirationDate = license.ExpirationDate,
            MaxActivations = license.MaxActivations,
            CurrentActivations = license.CurrentActivations,
            Status = license.Status,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }

    public async Task<ServiceResult<LicenseDto>> GetLicenseByIdAsync(Guid id)
    {
        try
        {
            var license = await _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (license == null)
            {
                return ServiceResult<LicenseDto>.Failure(
                    $"License with ID {id} not found",
                    "NOT_FOUND");
            }

            return ServiceResult<LicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving license with ID: {LicenseId}", id);
            return ServiceResult<LicenseDto>.Failure(
                "An error occurred while retrieving the license",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<LicenseDto>> UpdateLicenseAsync(Guid id, UpdateLicenseRequest request)
    {
        try
        {
            var license = await _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (license == null)
            {
                return ServiceResult<LicenseDto>.Failure(
                    $"License with ID {id} not found",
                    "NOT_FOUND");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.LicenseType))
            {
                license.LicenseType = request.LicenseType;
            }

            if (request.ExpirationDate.HasValue)
            {
                license.ExpirationDate = request.ExpirationDate;
            }

            if (request.MaxActivations.HasValue)
            {
                license.MaxActivations = request.MaxActivations.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("License updated with ID: {LicenseId}", id);

            return ServiceResult<LicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating license with ID: {LicenseId}", id);
            return ServiceResult<LicenseDto>.Failure(
                "An error occurred while updating the license",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteLicenseAsync(Guid id)
    {
        try
        {
            var license = await _context.Licenses.FindAsync(id);

            if (license == null)
            {
                return ServiceResult<bool>.Failure(
                    $"License with ID {id} not found",
                    "NOT_FOUND");
            }

            _context.Licenses.Remove(license);
            await _context.SaveChangesAsync();

            _logger.LogInformation("License deleted with ID: {LicenseId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting license with ID: {LicenseId}", id);
            return ServiceResult<bool>.Failure(
                "An error occurred while deleting the license",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> RevokeLicenseAsync(Guid id)
    {
        try
        {
            var license = await _context.Licenses.FindAsync(id);

            if (license == null)
            {
                return ServiceResult<bool>.Failure(
                    $"License with ID {id} not found",
                    "NOT_FOUND");
            }

            license.Status = LicenseStatus.Revoked;
            await _context.SaveChangesAsync();

            _logger.LogInformation("License revoked with ID: {LicenseId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking license with ID: {LicenseId}", id);
            return ServiceResult<bool>.Failure(
                "An error occurred while revoking the license",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var licenses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var licenseDtos = licenses.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<LicenseDto>(licenseDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<LicenseDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing licenses");
            return ServiceResult<PagedResult<LicenseDto>>.Failure(
                "An error occurred while listing licenses",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByCustomerAsync(Guid customerId, int page, int pageSize)
    {
        try
        {
            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
            {
                return ServiceResult<PagedResult<LicenseDto>>.Failure(
                    $"Customer with ID {customerId} not found",
                    "NOT_FOUND");
            }

            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .Where(l => l.CustomerId == customerId)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var licenses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var licenseDtos = licenses.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<LicenseDto>(licenseDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<LicenseDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing licenses for customer: {CustomerId}", customerId);
            return ServiceResult<PagedResult<LicenseDto>>.Failure(
                "An error occurred while listing licenses",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByProductAsync(Guid productId, int page, int pageSize)
    {
        try
        {
            // Validate product exists
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
            {
                return ServiceResult<PagedResult<LicenseDto>>.Failure(
                    $"Product with ID {productId} not found",
                    "NOT_FOUND");
            }

            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .Where(l => l.ProductId == productId)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var licenses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var licenseDtos = licenses.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<LicenseDto>(licenseDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<LicenseDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing licenses for product: {ProductId}", productId);
            return ServiceResult<PagedResult<LicenseDto>>.Failure(
                "An error occurred while listing licenses",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByStatusAsync(LicenseStatus status, int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .Where(l => l.Status == status)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var licenses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var licenseDtos = licenses.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<LicenseDto>(licenseDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<LicenseDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing licenses by status: {Status}", status);
            return ServiceResult<PagedResult<LicenseDto>>.Failure(
                "An error occurred while listing licenses",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<LicenseValidationResult>> ValidateLicenseKeyAsync(string licenseKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
                {
                    IsValid = false,
                    Reason = "License key is required"
                });
            }

            // Find license by comparing hashes
            var licenses = await _context.Licenses
                .Include(l => l.Customer)
                .Include(l => l.Product)
                .Include(l => l.Sku)
                .Include(l => l.RsaKey)
                .ToListAsync();

            License? matchedLicense = null;
            foreach (var license in licenses)
            {
                if (_keyGenerator.VerifyLicenseKey(licenseKey, license.LicenseKeyHash))
                {
                    matchedLicense = license;
                    break;
                }
            }

            if (matchedLicense == null)
            {
                return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
                {
                    IsValid = false,
                    Reason = "License key not found"
                });
            }

            // Check if license is revoked
            if (matchedLicense.Status == LicenseStatus.Revoked)
            {
                return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
                {
                    IsValid = false,
                    Reason = "License has been revoked",
                    License = MapToDto(matchedLicense)
                });
            }

            // Check if license has expired
            if (matchedLicense.ExpirationDate.HasValue && 
                matchedLicense.ExpirationDate.Value < DateTime.UtcNow)
            {
                // Update status to expired if not already
                if (matchedLicense.Status != LicenseStatus.Expired)
                {
                    matchedLicense.Status = LicenseStatus.Expired;
                    await _context.SaveChangesAsync();
                }

                return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
                {
                    IsValid = false,
                    Reason = "License has expired",
                    License = MapToDto(matchedLicense)
                });
            }

            // Check activation count
            if (matchedLicense.CurrentActivations >= matchedLicense.MaxActivations)
            {
                return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
                {
                    IsValid = false,
                    Reason = "Maximum activation count reached",
                    License = MapToDto(matchedLicense)
                });
            }

            return ServiceResult<LicenseValidationResult>.Success(new LicenseValidationResult
            {
                IsValid = true,
                License = MapToDto(matchedLicense)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating license key");
            return ServiceResult<LicenseValidationResult>.Failure(
                "An error occurred while validating the license key",
                "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<LicenseActivationResult>> ActivateLicenseKeyAsync(string licenseKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                return ServiceResult<LicenseActivationResult>.Success(new LicenseActivationResult
                {
                    IsActivated = false,
                    Reason = "License key is required"
                });
            }

            // Validate the license first
            var validationResult = await ValidateLicenseKeyAsync(licenseKey);
            if (!validationResult.IsSuccess)
            {
                return ServiceResult<LicenseActivationResult>.Failure(
                    validationResult.ErrorMessage ?? "Validation failed",
                    validationResult.ErrorCode);
            }

            if (!validationResult.Data!.IsValid)
            {
                return ServiceResult<LicenseActivationResult>.Success(new LicenseActivationResult
                {
                    IsActivated = false,
                    Reason = validationResult.Data.Reason
                });
            }

            // Find the license again to increment activation count
            var licenses = await _context.Licenses.ToListAsync();
            License? matchedLicense = null;
            foreach (var license in licenses)
            {
                if (_keyGenerator.VerifyLicenseKey(licenseKey, license.LicenseKeyHash))
                {
                    matchedLicense = license;
                    break;
                }
            }

            if (matchedLicense == null)
            {
                return ServiceResult<LicenseActivationResult>.Success(new LicenseActivationResult
                {
                    IsActivated = false,
                    Reason = "License key not found"
                });
            }

            // Increment activation count
            matchedLicense.CurrentActivations++;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "License activated. ID: {LicenseId}, Activations: {Current}/{Max}",
                matchedLicense.Id,
                matchedLicense.CurrentActivations,
                matchedLicense.MaxActivations);

            return ServiceResult<LicenseActivationResult>.Success(new LicenseActivationResult
            {
                IsActivated = true,
                CurrentActivations = matchedLicense.CurrentActivations,
                MaxActivations = matchedLicense.MaxActivations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating license key");
            return ServiceResult<LicenseActivationResult>.Failure(
                "An error occurred while activating the license key",
                "INTERNAL_ERROR");
        }
    }
}

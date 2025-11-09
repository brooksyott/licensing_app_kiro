using LicenseManagementApi.Models.Entities;

namespace LicenseManagementApi.Services;

public interface ILicenseKeyGenerator
{
    /// <summary>
    /// Generates a cryptographically secure license key with minimum 20 character length
    /// </summary>
    /// <returns>A unique license key string</returns>
    string GenerateLicenseKey();

    /// <summary>
    /// Hashes a license key with salt for secure storage
    /// </summary>
    /// <param name="licenseKey">The license key to hash</param>
    /// <returns>The hashed license key</returns>
    string HashLicenseKey(string licenseKey);

    /// <summary>
    /// Verifies a license key against its hash using constant-time comparison
    /// </summary>
    /// <param name="licenseKey">The license key to verify</param>
    /// <param name="hash">The hash to compare against</param>
    /// <returns>True if the key matches the hash, false otherwise</returns>
    bool VerifyLicenseKey(string licenseKey, string hash);

    /// <summary>
    /// Generates a JWT-based license key containing license and SKU information
    /// </summary>
    /// <param name="license">The license entity</param>
    /// <param name="skus">List of SKUs associated with the license</param>
    /// <returns>A JWT-encoded license key</returns>
    Task<string> GenerateAsync(License license, List<Sku> skus);
}

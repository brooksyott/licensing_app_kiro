using System.Security.Cryptography;
using System.Text;

namespace LicenseManagementApi.Services;

public class LicenseKeyGenerator : ILicenseKeyGenerator
{
    private const int MinimumKeyLength = 20;
    private const int DefaultKeyLength = 32;
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateLicenseKey()
    {
        var keyBytes = new byte[DefaultKeyLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }

        var result = new StringBuilder(DefaultKeyLength);
        foreach (var b in keyBytes)
        {
            result.Append(AllowedCharacters[b % AllowedCharacters.Length]);
        }

        var licenseKey = result.ToString();
        
        // Format as XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XX for readability
        return FormatLicenseKey(licenseKey);
    }

    public string HashLicenseKey(string licenseKey)
    {
        // Use PBKDF2 with SHA256 for secure hashing with salt
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(
            licenseKey,
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

    public bool VerifyLicenseKey(string licenseKey, string hash)
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
                licenseKey,
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

    private string FormatLicenseKey(string key)
    {
        // Format as groups of 5 characters separated by hyphens
        var formatted = new StringBuilder();
        for (int i = 0; i < key.Length; i++)
        {
            if (i > 0 && i % 5 == 0)
            {
                formatted.Append('-');
            }
            formatted.Append(key[i]);
        }
        return formatted.ToString();
    }
}

using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementApi.Services;

public class LicenseKeyGenerator : ILicenseKeyGenerator
{
    private const int MinimumKeyLength = 20;
    private const int DefaultKeyLength = 32;
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    private readonly LicenseManagementDbContext _context;
    private readonly ICryptographyService _cryptographyService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LicenseKeyGenerator> _logger;

    public LicenseKeyGenerator(
        LicenseManagementDbContext context,
        ICryptographyService cryptographyService,
        IConfiguration configuration,
        ILogger<LicenseKeyGenerator> logger)
    {
        _context = context;
        _cryptographyService = cryptographyService;
        _configuration = configuration;
        _logger = logger;
    }

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

    public async Task<string> GenerateAsync(License license, List<Sku> skus)
    {
        try
        {
            // Load related entities if not already loaded
            if (license.Customer == null)
            {
                await _context.Entry(license)
                    .Reference(l => l.Customer)
                    .LoadAsync();
            }

            if (license.RsaKey == null)
            {
                await _context.Entry(license)
                    .Reference(l => l.RsaKey)
                    .LoadAsync();
            }

            // Ensure SKUs have their Product navigation property loaded
            foreach (var sku in skus)
            {
                if (sku.Product == null)
                {
                    await _context.Entry(sku)
                        .Reference(s => s.Product)
                        .LoadAsync();
                }
            }

            // Group SKUs by product
            var productGroups = skus
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Product = g.First().Product,
                    Skus = g.ToList()
                })
                .ToList();

            // Build license name from license type and customer name
            if (license.Customer == null)
            {
                throw new InvalidOperationException("License customer is not loaded");
            }
            var licenseName = $"{license.LicenseType} License - {license.Customer.Name}";

            // Create products array with product and SKU details using dictionaries for JWT serialization
            var productsPayload = productGroups.Select(pg => new Dictionary<string, object>
            {
                { "productId", pg.ProductId.ToString() },
                { "productName", pg.Product.Name },
                { "productCode", pg.Product.ProductCode },
                { "skus", pg.Skus.Select(s => new Dictionary<string, object>
                    {
                        { "skuId", s.Id.ToString() },
                        { "skuName", s.Name },
                        { "skuCode", s.SkuCode }
                    }).ToList()
                }
            }).ToList();

            // Build JWT payload with license name and products array
            var now = DateTimeOffset.UtcNow;
            var expirationTime = license.ExpirationDate.HasValue 
                ? new DateTimeOffset(license.ExpirationDate.Value) 
                : now.AddYears(100); // Far future if no expiration

            var licensePayload = new Dictionary<string, object>
            {
                { "id", license.Id.ToString() },
                { "name", licenseName },
                { "type", license.LicenseType },
                { "status", license.Status.ToString() },
                { "maxActivations", license.MaxActivations },
                { "products", productsPayload }
            };

            var claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Iss, "LicenseManagementSystem" },
                { JwtRegisteredClaimNames.Sub, license.CustomerId.ToString() },
                { JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds() },
                { JwtRegisteredClaimNames.Exp, expirationTime.ToUnixTimeSeconds() },
                { "license", licensePayload }
            };

            // Get the RSA private key passphrase from configuration
            var passphrase = _configuration["Cryptography:PrivateKeyPassphrase"] 
                ?? _configuration["RsaKey:Passphrase"]
                ?? throw new InvalidOperationException("RSA key passphrase not configured");

            // Decrypt the private key
            if (license.RsaKey == null)
            {
                throw new InvalidOperationException("License RSA key is not loaded");
            }
            var privateKeyBase64 = _cryptographyService.DecryptPrivateKey(
                license.RsaKey.PrivateKeyEncrypted, 
                passphrase);

            // Import RSA private key
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

            // Create signing credentials
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256);

            // Create JWT token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                SigningCredentials = signingCredentials
            };

            // Sign and encode JWT with RSA key
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            _logger.LogInformation(
                "Generated JWT license key for license {LicenseId} with {SkuCount} SKUs",
                license.Id,
                skus.Count);

            return jwt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT license key for license {LicenseId}", license.Id);
            throw;
        }
    }
}

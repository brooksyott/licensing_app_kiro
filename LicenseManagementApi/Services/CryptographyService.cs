using System.Security.Cryptography;
using System.Text;

namespace LicenseManagementApi.Services;

public class CryptographyService : ICryptographyService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CryptographyService> _logger;

    public CryptographyService(IConfiguration configuration, ILogger<CryptographyService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public (string publicKey, string privateKey) GenerateRsaKeyPair(int keySize)
    {
        try
        {
            using var rsa = RSA.Create(keySize);
            
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

            _logger.LogInformation("Generated RSA key pair with key size {KeySize}", keySize);
            
            return (publicKey, privateKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating RSA key pair with key size {KeySize}", keySize);
            throw;
        }
    }

    public string EncryptPrivateKey(string privateKey, string passphrase)
    {
        try
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Derive key from passphrase using PBKDF2
            var salt = RandomNumberGenerator.GetBytes(32);
            using var pbkdf2 = new Rfc2898DeriveBytes(passphrase, salt, 100000, HashAlgorithmName.SHA256);
            aes.Key = pbkdf2.GetBytes(32);
            aes.IV = RandomNumberGenerator.GetBytes(16);

            using var encryptor = aes.CreateEncryptor();
            var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            var encryptedBytes = encryptor.TransformFinalBlock(privateKeyBytes, 0, privateKeyBytes.Length);

            // Combine salt + IV + encrypted data
            var result = new byte[salt.Length + aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(aes.IV, 0, result, salt.Length, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, salt.Length + aes.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting private key");
            throw;
        }
    }

    public string DecryptPrivateKey(string encryptedPrivateKey, string passphrase)
    {
        try
        {
            var encryptedData = Convert.FromBase64String(encryptedPrivateKey);

            // Extract salt, IV, and encrypted bytes
            var salt = new byte[32];
            var iv = new byte[16];
            var encryptedBytes = new byte[encryptedData.Length - 48];

            Buffer.BlockCopy(encryptedData, 0, salt, 0, 32);
            Buffer.BlockCopy(encryptedData, 32, iv, 0, 16);
            Buffer.BlockCopy(encryptedData, 48, encryptedBytes, 0, encryptedBytes.Length);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Derive key from passphrase using PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(passphrase, salt, 100000, HashAlgorithmName.SHA256);
            aes.Key = pbkdf2.GetBytes(32);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting private key");
            throw;
        }
    }

    public string SignData(string data, string privateKey)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            _logger.LogInformation("Signed data successfully");
            
            return Convert.ToBase64String(signature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing data");
            throw;
        }
    }

    public bool VerifySignature(string data, string signature, string publicKey)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);

            var isValid = rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            _logger.LogInformation("Signature verification result: {IsValid}", isValid);
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying signature");
            return false;
        }
    }
}

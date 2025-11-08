namespace LicenseManagementApi.Services;

public interface ICryptographyService
{
    /// <summary>
    /// Generates a new RSA key pair with the specified key size
    /// </summary>
    /// <param name="keySize">The size of the key in bits (2048 or 4096)</param>
    /// <returns>Tuple containing (publicKey, privateKey)</returns>
    (string publicKey, string privateKey) GenerateRsaKeyPair(int keySize);

    /// <summary>
    /// Encrypts the private key using AES encryption
    /// </summary>
    /// <param name="privateKey">The private key to encrypt</param>
    /// <param name="passphrase">The passphrase to use for encryption</param>
    /// <returns>The encrypted private key</returns>
    string EncryptPrivateKey(string privateKey, string passphrase);

    /// <summary>
    /// Decrypts the private key using AES decryption
    /// </summary>
    /// <param name="encryptedPrivateKey">The encrypted private key</param>
    /// <param name="passphrase">The passphrase to use for decryption</param>
    /// <returns>The decrypted private key</returns>
    string DecryptPrivateKey(string encryptedPrivateKey, string passphrase);

    /// <summary>
    /// Signs license data using RSA private key
    /// </summary>
    /// <param name="data">The data to sign</param>
    /// <param name="privateKey">The private key to use for signing</param>
    /// <returns>The digital signature as a base64 string</returns>
    string SignData(string data, string privateKey);

    /// <summary>
    /// Verifies a signature using RSA public key
    /// </summary>
    /// <param name="data">The original data</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key to use for verification</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    bool VerifySignature(string data, string signature, string publicKey);
}

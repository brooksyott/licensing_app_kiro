using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class CryptographyServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<CryptographyService>> _loggerMock;

    public CryptographyServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<CryptographyService>>();
    }

    [Fact]
    public void GenerateRsaKeyPair_ValidKeySize_ReturnsKeyPair()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);

        // Act
        var (publicKey, privateKey) = service.GenerateRsaKeyPair(2048);

        // Assert
        Assert.NotNull(publicKey);
        Assert.NotNull(privateKey);
        Assert.NotEmpty(publicKey);
        Assert.NotEmpty(privateKey);
    }

    [Fact]
    public void EncryptPrivateKey_ValidInput_ReturnsEncryptedKey()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);
        var privateKey = "test-private-key";
        var passphrase = "test-passphrase";

        // Act
        var encryptedKey = service.EncryptPrivateKey(privateKey, passphrase);

        // Assert
        Assert.NotNull(encryptedKey);
        Assert.NotEmpty(encryptedKey);
        Assert.NotEqual(privateKey, encryptedKey);
    }

    [Fact]
    public void DecryptPrivateKey_ValidInput_ReturnsOriginalKey()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);
        var originalKey = "test-private-key";
        var passphrase = "test-passphrase";
        var encryptedKey = service.EncryptPrivateKey(originalKey, passphrase);

        // Act
        var decryptedKey = service.DecryptPrivateKey(encryptedKey, passphrase);

        // Assert
        Assert.Equal(originalKey, decryptedKey);
    }

    [Fact]
    public void SignData_ValidInput_ReturnsSignature()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);
        var (publicKey, privateKey) = service.GenerateRsaKeyPair(2048);
        var data = "test data to sign";

        // Act
        var signature = service.SignData(data, privateKey);

        // Assert
        Assert.NotNull(signature);
        Assert.NotEmpty(signature);
    }

    [Fact]
    public void VerifySignature_ValidSignature_ReturnsTrue()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);
        var (publicKey, privateKey) = service.GenerateRsaKeyPair(2048);
        var data = "test data to sign";
        var signature = service.SignData(data, privateKey);

        // Act
        var isValid = service.VerifySignature(data, signature, publicKey);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifySignature_InvalidSignature_ReturnsFalse()
    {
        // Arrange
        var service = new CryptographyService(_configurationMock.Object, _loggerMock.Object);
        var (publicKey, privateKey) = service.GenerateRsaKeyPair(2048);
        var data = "test data to sign";
        var signature = service.SignData(data, privateKey);
        var tamperedData = "tampered data";

        // Act
        var isValid = service.VerifySignature(tamperedData, signature, publicKey);

        // Assert
        Assert.False(isValid);
    }
}

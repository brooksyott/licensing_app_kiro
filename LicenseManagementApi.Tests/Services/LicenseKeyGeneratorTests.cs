using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class LicenseKeyGeneratorTests
{
    private readonly LicenseKeyGenerator _generator;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<LicenseKeyGenerator>> _loggerMock;

    public LicenseKeyGeneratorTests()
    {
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<LicenseKeyGenerator>>();
        
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new LicenseManagementDbContext(options);
        
        _generator = new LicenseKeyGenerator(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateLicenseKey_ReturnsNonEmptyString()
    {
        // Act
        var licenseKey = _generator.GenerateLicenseKey();

        // Assert
        Assert.NotNull(licenseKey);
        Assert.NotEmpty(licenseKey);
    }

    [Fact]
    public void GenerateLicenseKey_ReturnsMinimumLength()
    {
        // Act
        var licenseKey = _generator.GenerateLicenseKey();
        var keyWithoutHyphens = licenseKey.Replace("-", "");

        // Assert
        Assert.True(keyWithoutHyphens.Length >= 20, "License key should be at least 20 characters");
    }

    [Fact]
    public void GenerateLicenseKey_GeneratesUniqueKeys()
    {
        // Act
        var key1 = _generator.GenerateLicenseKey();
        var key2 = _generator.GenerateLicenseKey();

        // Assert
        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void HashLicenseKey_ReturnsNonEmptyHash()
    {
        // Arrange
        var licenseKey = "TEST-12345-67890";

        // Act
        var hash = _generator.HashLicenseKey(licenseKey);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashLicenseKey_SameKeyProducesDifferentHashes()
    {
        // Arrange
        var licenseKey = "TEST-12345-67890";

        // Act
        var hash1 = _generator.HashLicenseKey(licenseKey);
        var hash2 = _generator.HashLicenseKey(licenseKey);

        // Assert - Different hashes due to different salts
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyLicenseKey_ValidKey_ReturnsTrue()
    {
        // Arrange
        var licenseKey = "TEST-12345-67890";
        var hash = _generator.HashLicenseKey(licenseKey);

        // Act
        var isValid = _generator.VerifyLicenseKey(licenseKey, hash);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyLicenseKey_InvalidKey_ReturnsFalse()
    {
        // Arrange
        var licenseKey = "TEST-12345-67890";
        var wrongKey = "WRONG-12345-67890";
        var hash = _generator.HashLicenseKey(licenseKey);

        // Act
        var isValid = _generator.VerifyLicenseKey(wrongKey, hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyLicenseKey_InvalidHash_ReturnsFalse()
    {
        // Arrange
        var licenseKey = "TEST-12345-67890";
        var invalidHash = "invalid-hash";

        // Act
        var isValid = _generator.VerifyLicenseKey(licenseKey, invalidHash);

        // Assert
        Assert.False(isValid);
    }
}

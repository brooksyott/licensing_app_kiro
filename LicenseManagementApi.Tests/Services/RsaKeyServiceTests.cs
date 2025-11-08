using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class RsaKeyServiceTests
{
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<RsaKeyService>> _loggerMock;

    public RsaKeyServiceTests()
    {
        _cryptographyServiceMock = new Mock<ICryptographyService>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<RsaKeyService>>();
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task GenerateKeyPairAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var passphrase = "test-passphrase";
        
        _configurationMock.Setup(c => c["Cryptography:PrivateKeyPassphrase"]).Returns(passphrase);
        _cryptographyServiceMock.Setup(c => c.GenerateRsaKeyPair(It.IsAny<int>()))
            .Returns(("public-key", "private-key"));
        _cryptographyServiceMock.Setup(c => c.EncryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("encrypted-private-key");

        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var request = new GenerateRsaKeyRequest
        {
            Name = "Test Key",
            KeySize = 2048,
            CreatedBy = "test-user"
        };

        // Act
        var result = await service.GenerateKeyPairAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.KeySize, result.Data.KeySize);
        Assert.Equal(request.CreatedBy, result.Data.CreatedBy);
    }

    [Fact]
    public async Task GetKeyByIdAsync_ExistingKey_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var keyId = Guid.NewGuid();
        
        context.RsaKeys.Add(new RsaKey
        {
            Id = keyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test-user"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetKeyByIdAsync(keyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(keyId, result.Data.Id);
    }

    [Fact]
    public async Task GetKeyByIdAsync_NonExistingKey_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await service.GetKeyByIdAsync(nonExistingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task DownloadPrivateKeyAsync_ExistingKey_ReturnsDecryptedKey()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var passphrase = "test-passphrase";
        var decryptedKey = "decrypted-private-key";
        
        _configurationMock.Setup(c => c["Cryptography:PrivateKeyPassphrase"]).Returns(passphrase);
        _cryptographyServiceMock.Setup(c => c.DecryptPrivateKey(It.IsAny<string>(), passphrase))
            .Returns(decryptedKey);

        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var keyId = Guid.NewGuid();
        
        context.RsaKeys.Add(new RsaKey
        {
            Id = keyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test-user"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DownloadPrivateKeyAsync(keyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(decryptedKey, result.Data);
    }

    [Fact]
    public async Task UpdateKeyAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var keyId = Guid.NewGuid();
        
        context.RsaKeys.Add(new RsaKey
        {
            Id = keyId,
            Name = "Original Name",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test-user"
        });
        await context.SaveChangesAsync();

        var updateRequest = new UpdateRsaKeyRequest
        {
            Name = "Updated Name"
        };

        // Act
        var result = await service.UpdateKeyAsync(keyId, updateRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Data.Name);
    }

    [Fact]
    public async Task DeleteKeyAsync_ExistingKey_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        var keyId = Guid.NewGuid();
        
        context.RsaKeys.Add(new RsaKey
        {
            Id = keyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test-user"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteKeyAsync(keyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        
        var deletedKey = await context.RsaKeys.FindAsync(keyId);
        Assert.Null(deletedKey);
    }

    [Fact]
    public async Task ListKeysAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new RsaKeyService(context, _cryptographyServiceMock.Object, _configurationMock.Object, _loggerMock.Object);
        
        for (int i = 0; i < 5; i++)
        {
            context.RsaKeys.Add(new RsaKey
            {
                Id = Guid.NewGuid(),
                Name = $"Key {i}",
                PublicKey = $"public-key-{i}",
                PrivateKeyEncrypted = $"encrypted-private-key-{i}",
                KeySize = 2048,
                CreatedBy = "test-user"
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListKeysAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(5, result.Data.Items.Count());
    }
}

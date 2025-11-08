using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class LicenseServiceTests
{
    private readonly Mock<ILogger<LicenseService>> _loggerMock;
    private readonly Mock<ILicenseKeyGenerator> _keyGeneratorMock;
    private readonly Mock<ICryptographyService> _cryptographyServiceMock;

    public LicenseServiceTests()
    {
        _loggerMock = new Mock<ILogger<LicenseService>>();
        _keyGeneratorMock = new Mock<ILicenseKeyGenerator>();
        _cryptographyServiceMock = new Mock<ICryptographyService>();
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task CreateLicenseAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.GenerateLicenseKey())
            .Returns("TEST-12345-67890-ABCDE-FGHIJ-KLMNO-PQ");
        _keyGeneratorMock.Setup(x => x.HashLicenseKey(It.IsAny<string>()))
            .Returns("hashed-key");
        _cryptographyServiceMock.Setup(x => x.DecryptPrivateKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decrypted-private-key");
        _cryptographyServiceMock.Setup(x => x.SignData(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("signed-payload");

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        var request = new CreateLicenseRequest
        {
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseType = "Standard",
            MaxActivations = 5
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(customerId, result.Data.CustomerId);
        Assert.Equal(productId, result.Data.ProductId);
        Assert.Equal("TEST-12345-67890-ABCDE-FGHIJ-KLMNO-PQ", result.Data.LicenseKey);
    }

    [Fact]
    public async Task CreateLicenseAsync_CustomerNotFound_ReturnsFailure()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        var request = new CreateLicenseRequest
        {
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            RsaKeyId = Guid.NewGuid(),
            LicenseType = "Standard",
            MaxActivations = 5
        };

        // Act
        var result = await service.CreateLicenseAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task GetLicenseByIdAsync_ExistingLicense_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = licenseId,
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = "TEST-KEY",
            LicenseKeyHash = "hash",
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.GetLicenseByIdAsync(licenseId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(licenseId, result.Data.Id);
    }

    [Fact]
    public async Task RevokeLicenseAsync_ExistingLicense_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseId = Guid.NewGuid();

        context.Licenses.Add(new License
        {
            Id = licenseId,
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            RsaKeyId = Guid.NewGuid(),
            LicenseKey = "TEST-KEY",
            LicenseKeyHash = "hash",
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.RevokeLicenseAsync(licenseId);

        // Assert
        Assert.True(result.IsSuccess);
        var license = await context.Licenses.FindAsync(licenseId);
        Assert.Equal(LicenseStatus.Revoked, license!.Status);
    }

    [Fact]
    public async Task ValidateLicenseKeyAsync_ValidKey_ReturnsValid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ValidateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data!.IsValid);
    }

    [Fact]
    public async Task ValidateLicenseKeyAsync_RevokedLicense_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Revoked
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ValidateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsValid);
        Assert.Contains("revoked", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateLicenseKeyAsync_ExpiredLicense_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            ExpirationDate = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ValidateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsValid);
        Assert.Contains("expired", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateLicenseKeyAsync_MaxActivationsReached_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 5, // Already at max
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ValidateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsValid);
        Assert.Contains("activation", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateLicenseKeyAsync_NonExistentKey_ReturnsInvalid()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "NONEXISTENT-KEY";

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ValidateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsValid);
        Assert.Contains("not found", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ActivateLicenseKeyAsync_ValidKey_IncrementsActivationCount()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var licenseId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = licenseId,
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 2,
            Status = LicenseStatus.Active
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ActivateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data!.IsActivated);
        Assert.Equal(3, result.Data.CurrentActivations);
        Assert.Equal(5, result.Data.MaxActivations);

        // Verify database was updated
        var license = await context.Licenses.FindAsync(licenseId);
        Assert.Equal(3, license!.CurrentActivations);
    }

    [Fact]
    public async Task ActivateLicenseKeyAsync_InvalidKey_ReturnsNotActivated()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "INVALID-KEY";

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ActivateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsActivated);
        Assert.Contains("not found", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ActivateLicenseKeyAsync_RevokedLicense_ReturnsNotActivated()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var licenseKey = "TEST-12345-67890";
        var licenseHash = "hashed-key";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rsaKeyId = Guid.NewGuid();

        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });

        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            ProductCode = "PROD-001",
            Version = "1.0"
        });

        context.RsaKeys.Add(new RsaKey
        {
            Id = rsaKeyId,
            Name = "Test Key",
            PublicKey = "public-key",
            PrivateKeyEncrypted = "encrypted-private-key",
            KeySize = 2048,
            CreatedBy = "test"
        });

        context.Licenses.Add(new License
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            RsaKeyId = rsaKeyId,
            LicenseKey = licenseKey,
            LicenseKeyHash = licenseHash,
            SignedPayload = "payload",
            LicenseType = "Standard",
            MaxActivations = 5,
            CurrentActivations = 0,
            Status = LicenseStatus.Revoked
        });

        await context.SaveChangesAsync();

        _keyGeneratorMock.Setup(x => x.VerifyLicenseKey(licenseKey, licenseHash))
            .Returns(true);

        var service = new LicenseService(
            context,
            _loggerMock.Object,
            _keyGeneratorMock.Object,
            _cryptographyServiceMock.Object);

        // Act
        var result = await service.ActivateLicenseKeyAsync(licenseKey);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsActivated);
        Assert.Contains("revoked", result.Data.Reason!, StringComparison.OrdinalIgnoreCase);
    }
}

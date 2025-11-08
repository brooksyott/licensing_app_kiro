# Testing Guide

This guide covers testing strategies and practices for the License Management API.

## Test Structure

The test project `LicenseManagementApi.Tests` contains unit tests for the service layer.

```
LicenseManagementApi.Tests/
├── Services/
│   ├── ApiKeyServiceTests.cs
│   ├── AuthenticationServiceTests.cs
│   ├── LicenseServiceTests.cs
│   └── LicenseKeyGeneratorTests.cs
└── LicenseManagementApi.Tests.csproj
```

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests with Detailed Output

```bash
dotnet test --verbosity detailed
```

### Run Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~ApiKeyServiceTests"
```

### Run Specific Test Method

```bash
dotnet test --filter "FullyQualifiedName~CreateApiKeyAsync_ValidRequest_ReturnsSuccess"
```

## Test Framework

The project uses:

- **xUnit** - Test framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library (optional)

## Testing Patterns

### Service Layer Testing

Service tests use mocked dependencies to isolate business logic:

```csharp
public class ApiKeyServiceTests
{
    private readonly Mock<LicenseManagementDbContext> _mockContext;
    private readonly Mock<ILogger<ApiKeyService>> _mockLogger;
    private readonly ApiKeyService _service;

    public ApiKeyServiceTests()
    {
        _mockContext = new Mock<LicenseManagementDbContext>();
        _mockLogger = new Mock<ILogger<ApiKeyService>>();
        _service = new ApiKeyService(_mockContext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateApiKeyAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateApiKeyRequest
        {
            Name = "Test Key",
            Role = "Admin",
            CreatedBy = "test@example.com"
        };

        // Act
        var result = await _service.CreateApiKeyAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Test Key", result.Data.Name);
    }
}
```

### Testing ServiceResult Pattern

All service methods return `ServiceResult<T>`:

```csharp
[Fact]
public async Task GetApiKeyByIdAsync_NotFound_ReturnsFailure()
{
    // Arrange
    var nonExistentId = Guid.NewGuid();

    // Act
    var result = await _service.GetApiKeyByIdAsync(nonExistentId);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    Assert.Equal("NOT_FOUND", result.ErrorCode);
}
```

### Testing Validation

Test validation logic for request models:

```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public async Task CreateApiKeyAsync_InvalidName_ReturnsValidationError(string invalidName)
{
    // Arrange
    var request = new CreateApiKeyRequest
    {
        Name = invalidName,
        Role = "Admin",
        CreatedBy = "test@example.com"
    };

    // Act
    var result = await _service.CreateApiKeyAsync(request);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ValidationErrors);
    Assert.Contains("Name", result.ValidationErrors.Keys);
}
```

## Test Coverage Goals

Target coverage levels:

- **Service Layer**: 80%+ coverage
- **Critical Paths**: 100% coverage (authentication, license validation, key generation)
- **Controllers**: Integration tests (future enhancement)

## Integration Testing

!!! note "Future Enhancement"
    Integration tests using WebApplicationFactory are planned for future releases.

Example integration test structure:

```csharp
public class CustomersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CustomersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSuccessStatusCode()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Auth_Key", "test-api-key");

        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

## Database Testing

### In-Memory Database

For unit tests, use in-memory database:

```csharp
private LicenseManagementDbContext CreateInMemoryContext()
{
    var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    return new LicenseManagementDbContext(options);
}
```

### Test Database

For integration tests, use a dedicated test database:

```json
{
  "ConnectionStrings": {
    "TestConnection": "Host=localhost;Database=LicenseManagement_Test;Username=postgres;Password=postgres"
  }
}
```

## Mocking Best Practices

### Mock DbContext

```csharp
var mockSet = new Mock<DbSet<Customer>>();
var mockContext = new Mock<LicenseManagementDbContext>();
mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
```

### Mock ILogger

```csharp
var mockLogger = new Mock<ILogger<CustomerService>>();
```

### Verify Method Calls

```csharp
_mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
```

## Test Data Builders

Create test data builders for complex objects:

```csharp
public class CustomerBuilder
{
    private string _name = "Test Customer";
    private string _email = "test@example.com";
    private string _organization = "Test Org";

    public CustomerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public Customer Build()
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = _name,
            Email = _email,
            Organization = _organization,
            IsVisible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

// Usage
var customer = new CustomerBuilder()
    .WithName("John Doe")
    .WithEmail("john@example.com")
    .Build();
```

## Testing Cryptography

### License Key Generation

```csharp
[Fact]
public void GenerateLicenseKey_ReturnsValidFormat()
{
    // Arrange
    var generator = new LicenseKeyGenerator();

    // Act
    var licenseKey = generator.GenerateLicenseKey();

    // Assert
    Assert.StartsWith("LIC_", licenseKey);
    Assert.Equal(44, licenseKey.Length); // LIC_ + 40 characters
}
```

### RSA Key Generation

```csharp
[Theory]
[InlineData(2048)]
[InlineData(4096)]
public void GenerateRsaKeyPair_ValidKeySize_ReturnsKeyPair(int keySize)
{
    // Arrange
    var generator = new RsaKeyGenerator();

    // Act
    var (publicKey, privateKey) = generator.GenerateKeyPair(keySize);

    // Assert
    Assert.Contains("BEGIN PUBLIC KEY", publicKey);
    Assert.Contains("BEGIN PRIVATE KEY", privateKey);
}
```

## Performance Testing

### Benchmark Tests

Use BenchmarkDotNet for performance testing:

```csharp
[MemoryDiagnoser]
public class LicenseValidationBenchmarks
{
    private readonly ILicenseService _service;

    [Benchmark]
    public async Task ValidateLicenseKey()
    {
        await _service.ValidateLicenseKeyAsync("LIC_test_key");
    }
}
```

## Continuous Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: LicenseManagement_Test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
```

## Test Organization

### Naming Conventions

- Test class: `{ClassName}Tests`
- Test method: `{MethodName}_{Scenario}_{ExpectedResult}`

Examples:
- `CreateCustomerAsync_ValidRequest_ReturnsSuccess`
- `GetCustomerByIdAsync_NotFound_ReturnsFailure`
- `ValidateLicenseKeyAsync_ExpiredLicense_ReturnsInvalid`

### Test Categories

Use traits to categorize tests:

```csharp
[Trait("Category", "Unit")]
public class ApiKeyServiceTests { }

[Trait("Category", "Integration")]
public class CustomersControllerIntegrationTests { }
```

Run specific categories:

```bash
dotnet test --filter "Category=Unit"
```

## Troubleshooting Tests

### Database Connection Issues

Ensure PostgreSQL is running for integration tests:

```bash
pg_isready -h localhost -p 5432
```

### Flaky Tests

Avoid time-dependent assertions:

```csharp
// Bad
Assert.Equal(DateTime.UtcNow, customer.CreatedAt);

// Good
Assert.True((DateTime.UtcNow - customer.CreatedAt).TotalSeconds < 1);
```

### Parallel Test Execution

xUnit runs tests in parallel by default. Disable for database tests:

```csharp
[Collection("Database collection")]
public class CustomerServiceTests { }
```

## Next Steps

- Review [Requirements](requirements.md) for feature specifications
- Explore [Contributing Guide](contributing.md) for development workflow
- Check [API Documentation](../api/overview.md) for endpoint testing

# Contributing Guide

Thank you for your interest in contributing to the License Management API!

## Development Workflow

### 1. Fork and Clone

```bash
git clone https://github.com/your-org/license-management-api.git
cd license-management-api
```

### 2. Create a Branch

```bash
git checkout -b feature/your-feature-name
```

Branch naming conventions:
- `feature/` - New features
- `bugfix/` - Bug fixes
- `hotfix/` - Critical production fixes
- `docs/` - Documentation updates

### 3. Make Changes

Follow the coding standards and best practices outlined below.

### 4. Test Your Changes

```bash
dotnet test
```

Ensure all tests pass before submitting.

### 5. Commit Your Changes

```bash
git add .
git commit -m "feat: add new feature description"
```

Follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `test:` - Test additions or changes
- `refactor:` - Code refactoring
- `chore:` - Maintenance tasks

### 6. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Create a pull request on GitHub with:
- Clear description of changes
- Reference to related issues
- Screenshots (if UI changes)
- Test results

## Coding Standards

### C# Style Guide

Follow Microsoft's C# coding conventions:

#### Naming Conventions

```csharp
// Classes, interfaces, methods: PascalCase
public class CustomerService { }
public interface ICustomerService { }
public async Task<ServiceResult<Customer>> GetCustomerAsync() { }

// Private fields: _camelCase
private readonly ILogger<CustomerService> _logger;

// Parameters, local variables: camelCase
public void ProcessCustomer(string customerName) { }

// Constants: PascalCase
public const int MaxPageSize = 100;
```

#### Code Organization

```csharp
public class CustomerService : ICustomerService
{
    // 1. Private fields
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    // 2. Constructor
    public CustomerService(
        LicenseManagementDbContext context,
        ILogger<CustomerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 3. Public methods
    public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        // Implementation
    }

    // 4. Private methods
    private async Task<bool> ValidateCustomerAsync(Customer customer)
    {
        // Implementation
    }
}
```

### Async/Await Guidelines

- Always use `async`/`await` for I/O operations
- Suffix async methods with `Async`
- Use `ConfigureAwait(false)` in library code (not required for ASP.NET Core)

```csharp
public async Task<ServiceResult<Customer>> GetCustomerAsync(Guid id)
{
    var customer = await _context.Customers
        .FirstOrDefaultAsync(c => c.Id == id);
    
    return ServiceResult<Customer>.Success(customer);
}
```

### Error Handling

Use the `ServiceResult<T>` pattern for all service methods:

```csharp
public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
{
    try
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<CustomerDto>.Failure("Name is required", "VALIDATION_ERROR");
        }

        // Business logic
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return ServiceResult<CustomerDto>.Success(MapToDto(customer));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating customer");
        return ServiceResult<CustomerDto>.Failure("An error occurred", "INTERNAL_ERROR");
    }
}
```

### Logging

Use structured logging with Serilog:

```csharp
// Information
_logger.LogInformation("Creating customer with email {Email}", request.Email);

// Warning
_logger.LogWarning("Customer {CustomerId} not found", id);

// Error
_logger.LogError(ex, "Error creating customer with email {Email}", request.Email);

// Debug
_logger.LogDebug("Validating customer data for {CustomerId}", id);
```

### Dependency Injection

Register services in `Program.cs`:

```csharp
// Scoped services (per request)
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Singleton services (application lifetime)
builder.Services.AddSingleton<ILicenseKeyGenerator, LicenseKeyGenerator>();

// Transient services (per injection)
builder.Services.AddTransient<IEmailService, EmailService>();
```

## Database Migrations

### Creating Migrations

```bash
dotnet ef migrations add MigrationName --project LicenseManagementApi
```

### Reviewing Migrations

Always review generated migration code before applying:

```csharp
public partial class AddCustomerOrganization : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Organization",
            table: "Customers",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Organization",
            table: "Customers");
    }
}
```

### Applying Migrations

```bash
dotnet ef database update --project LicenseManagementApi
```

## Testing Requirements

### Unit Tests

All new features must include unit tests:

```csharp
[Fact]
public async Task CreateCustomerAsync_ValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new CreateCustomerRequest
    {
        Name = "Test Customer",
        Email = "test@example.com"
    };

    // Act
    var result = await _service.CreateCustomerAsync(request);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Data);
}
```

### Test Coverage

- Aim for 80%+ coverage on service layer
- 100% coverage on critical paths (authentication, license validation)

### Running Tests

```bash
# All tests
dotnet test

# With coverage
dotnet test /p:CollectCoverage=true

# Specific test
dotnet test --filter "FullyQualifiedName~CreateCustomerAsync_ValidRequest_ReturnsSuccess"
```

## Documentation

### Code Comments

Use XML documentation comments for public APIs:

```csharp
/// <summary>
/// Creates a new customer in the system.
/// </summary>
/// <param name="request">The customer creation request containing name, email, and organization.</param>
/// <returns>A service result containing the created customer DTO or an error.</returns>
public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
{
    // Implementation
}
```

### API Documentation

Update Swagger/OpenAPI documentation for new endpoints:

```csharp
[HttpPost]
[ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
{
    // Implementation
}
```

### README Updates

Update README.md for:
- New features
- Configuration changes
- Breaking changes
- New dependencies

## Pull Request Guidelines

### PR Title

Use conventional commit format:

```
feat: add customer organization field
fix: resolve license validation bug
docs: update API documentation
```

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests pass locally
- [ ] Dependent changes merged

## Related Issues
Closes #123
```

### Code Review Process

1. Automated checks must pass (build, tests, linting)
2. At least one approval required
3. Address all review comments
4. Squash commits before merging

## Development Environment

### Required Tools

- .NET 8.0 SDK
- PostgreSQL 12+
- Git
- IDE (Visual Studio, VS Code, or Rider)

### Recommended Extensions

#### VS Code
- C# Dev Kit
- GitLens
- REST Client
- PostgreSQL

#### Visual Studio
- ReSharper (optional)
- CodeMaid (optional)

### Environment Setup

1. Install prerequisites
2. Clone repository
3. Configure database connection
4. Run migrations
5. Start application

```bash
git clone https://github.com/your-org/license-management-api.git
cd license-management-api
dotnet restore
dotnet ef database update --project LicenseManagementApi
dotnet run --project LicenseManagementApi
```

## Release Process

### Versioning

Follow [Semantic Versioning](https://semver.org/):

- MAJOR: Breaking changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes (backward compatible)

### Release Checklist

- [ ] All tests passing
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Version number bumped
- [ ] Migration scripts tested
- [ ] Security review completed
- [ ] Performance testing completed

## Getting Help

### Resources

- [API Documentation](../api/overview.md)
- [Architecture Guide](../architecture/design.md)
- [Testing Guide](testing.md)

### Communication

- GitHub Issues - Bug reports and feature requests
- GitHub Discussions - Questions and discussions
- Pull Requests - Code contributions

## Code of Conduct

### Our Standards

- Be respectful and inclusive
- Accept constructive criticism
- Focus on what's best for the community
- Show empathy towards others

### Unacceptable Behavior

- Harassment or discrimination
- Trolling or insulting comments
- Public or private harassment
- Publishing others' private information

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

## Recognition

Contributors will be recognized in:
- CONTRIBUTORS.md file
- Release notes
- Project documentation

Thank you for contributing to the License Management API!

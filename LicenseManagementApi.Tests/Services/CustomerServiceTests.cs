using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ILogger<CustomerService>> _loggerMock;

    public CustomerServiceTests()
    {
        _loggerMock = new Mock<ILogger<CustomerService>>();
    }

    private LicenseManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LicenseManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LicenseManagementDbContext(options);
    }

    [Fact]
    public async Task CreateCustomerAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        var request = new CreateCustomerRequest
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Organization = "Test Org",
            IsVisible = true
        };

        // Act
        var result = await service.CreateCustomerAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.Email, result.Data.Email);
    }

    [Fact]
    public async Task CreateCustomerAsync_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        
        context.Customers.Add(new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Existing Customer",
            Email = "test@example.com"
        });
        await context.SaveChangesAsync();

        var request = new CreateCustomerRequest
        {
            Name = "New Customer",
            Email = "test@example.com"
        };

        // Act
        var result = await service.CreateCustomerAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ExistingCustomer_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        var customerId = Guid.NewGuid();
        
        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(customerId, result.Data.Id);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_NonExistingCustomer_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await service.GetCustomerByIdAsync(nonExistingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        var customerId = Guid.NewGuid();
        
        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Original Name",
            Email = "original@example.com"
        });
        await context.SaveChangesAsync();

        var updateRequest = new UpdateCustomerRequest
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            IsVisible = true
        };

        // Act
        var result = await service.UpdateCustomerAsync(customerId, updateRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Equal("updated@example.com", result.Data.Email);
    }

    [Fact]
    public async Task DeleteCustomerAsync_ExistingCustomer_ReturnsSuccess()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        var customerId = Guid.NewGuid();
        
        context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteCustomerAsync(customerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        
        var deletedCustomer = await context.Customers.FindAsync(customerId);
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task ListCustomersAsync_ReturnsPagedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new CustomerService(context, _loggerMock.Object);
        
        for (int i = 0; i < 5; i++)
        {
            context.Customers.Add(new Customer
            {
                Id = Guid.NewGuid(),
                Name = $"Customer {i}",
                Email = $"customer{i}@example.com"
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.ListCustomersAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(5, result.Data.Items.Count());
    }
}

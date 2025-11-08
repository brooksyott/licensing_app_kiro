using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public class CustomerService : ICustomerService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(LicenseManagementDbContext context, ILogger<CustomerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            // Check for duplicate email
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (existingCustomer != null)
            {
                return ServiceResult<CustomerDto>.Failure(
                    "A customer with this email already exists",
                    "VALIDATION_ERROR");
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Organization = request.Organization,
                IsVisible = request.IsVisible
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer created with ID: {CustomerId}", customer.Id);

            return ServiceResult<CustomerDto>.Success(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return ServiceResult<CustomerDto>.Failure("An error occurred while creating the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<CustomerDto>> GetCustomerByIdAsync(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return ServiceResult<CustomerDto>.Failure($"Customer with ID {id} not found", "NOT_FOUND");
            }

            return ServiceResult<CustomerDto>.Success(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", id);
            return ServiceResult<CustomerDto>.Failure("An error occurred while retrieving the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<CustomerDto>> GetCustomerByNameAsync(string name)
    {
        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Name == name);

            if (customer == null)
            {
                return ServiceResult<CustomerDto>.Failure($"Customer with name '{name}' not found", "NOT_FOUND");
            }

            return ServiceResult<CustomerDto>.Success(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with name: {CustomerName}", name);
            return ServiceResult<CustomerDto>.Failure("An error occurred while retrieving the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<IEnumerable<CustomerDto>>> SearchCustomersAsync(string searchTerm, int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => 
                    c.Name.Contains(searchTerm) || 
                    c.Email.Contains(searchTerm) ||
                    (c.Organization != null && c.Organization.Contains(searchTerm)));
            }

            var customers = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerDtos = customers.Select(MapToDto);

            return ServiceResult<IEnumerable<CustomerDto>>.Success(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
            return ServiceResult<IEnumerable<CustomerDto>>.Failure("An error occurred while searching customers", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return ServiceResult<CustomerDto>.Failure($"Customer with ID {id} not found", "NOT_FOUND");
            }

            // Check for duplicate email (excluding current customer)
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == request.Email && c.Id != id);

            if (existingCustomer != null)
            {
                return ServiceResult<CustomerDto>.Failure(
                    "A customer with this email already exists",
                    "VALIDATION_ERROR");
            }

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Organization = request.Organization;
            customer.IsVisible = request.IsVisible;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer updated with ID: {CustomerId}", customer.Id);

            return ServiceResult<CustomerDto>.Success(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", id);
            return ServiceResult<CustomerDto>.Failure("An error occurred while updating the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteCustomerAsync(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return ServiceResult<bool>.Failure($"Customer with ID {id} not found", "NOT_FOUND");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer deleted with ID: {CustomerId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteCustomerByNameAsync(string name)
    {
        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Name == name);

            if (customer == null)
            {
                return ServiceResult<bool>.Failure($"Customer with name '{name}' not found", "NOT_FOUND");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer deleted with name: {CustomerName}", name);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with name: {CustomerName}", name);
            return ServiceResult<bool>.Failure("An error occurred while deleting the customer", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<CustomerDto>>> ListCustomersAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.Customers.CountAsync();

            var customers = await _context.Customers
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerDtos = customers.Select(MapToDto);

            var pagedResult = new PagedResult<CustomerDto>(customerDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<CustomerDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing customers");
            return ServiceResult<PagedResult<CustomerDto>>.Failure("An error occurred while listing customers", "INTERNAL_ERROR");
        }
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Organization = customer.Organization,
            IsVisible = customer.IsVisible,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

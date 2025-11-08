using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface ICustomerService
{
    Task<ServiceResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request);
    Task<ServiceResult<CustomerDto>> GetCustomerByIdAsync(Guid id);
    Task<ServiceResult<CustomerDto>> GetCustomerByNameAsync(string name);
    Task<ServiceResult<IEnumerable<CustomerDto>>> SearchCustomersAsync(string searchTerm, int page, int pageSize);
    Task<ServiceResult<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request);
    Task<ServiceResult<bool>> DeleteCustomerAsync(Guid id);
    Task<ServiceResult<bool>> DeleteCustomerByNameAsync(string name);
    Task<ServiceResult<PagedResult<CustomerDto>>> ListCustomersAsync(int page, int pageSize);
}

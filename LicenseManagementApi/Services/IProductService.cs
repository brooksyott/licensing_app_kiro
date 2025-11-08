using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface IProductService
{
    Task<ServiceResult<ProductDto>> CreateProductAsync(CreateProductRequest request);
    Task<ServiceResult<ProductDto>> GetProductByIdAsync(Guid id);
    Task<ServiceResult<ProductDto>> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<ServiceResult<bool>> DeleteProductAsync(Guid id);
    Task<ServiceResult<PagedResult<ProductDto>>> ListProductsAsync(int page, int pageSize);
    Task<ServiceResult<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm, int page, int pageSize);
}

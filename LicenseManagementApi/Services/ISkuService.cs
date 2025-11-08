using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface ISkuService
{
    Task<ServiceResult<SkuDto>> CreateSkuAsync(CreateSkuRequest request);
    Task<ServiceResult<SkuDto>> GetSkuByIdAsync(Guid id);
    Task<ServiceResult<SkuDto>> UpdateSkuAsync(Guid id, UpdateSkuRequest request);
    Task<ServiceResult<bool>> DeleteSkuAsync(Guid id);
    Task<ServiceResult<PagedResult<SkuDto>>> ListSkusAsync(int page, int pageSize);
    Task<ServiceResult<PagedResult<SkuDto>>> ListSkusByProductAsync(Guid productId, int page, int pageSize);
    Task<ServiceResult<IEnumerable<SkuDto>>> SearchSkusAsync(string searchTerm, int page, int pageSize);
}

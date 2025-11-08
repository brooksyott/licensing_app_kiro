using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface IRsaKeyService
{
    Task<ServiceResult<RsaKeyDto>> GenerateKeyPairAsync(GenerateRsaKeyRequest request);
    Task<ServiceResult<RsaKeyDto>> GetKeyByIdAsync(Guid id);
    Task<ServiceResult<string>> DownloadPrivateKeyAsync(Guid id);
    Task<ServiceResult<RsaKeyDto>> UpdateKeyAsync(Guid id, UpdateRsaKeyRequest request);
    Task<ServiceResult<bool>> DeleteKeyAsync(Guid id);
    Task<ServiceResult<PagedResult<RsaKeyDto>>> ListKeysAsync(int page, int pageSize);
}

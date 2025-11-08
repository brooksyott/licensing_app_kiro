using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface IApiKeyService
{
    Task<ServiceResult<ApiKeyDto>> CreateApiKeyAsync(CreateApiKeyRequest request);
    Task<ServiceResult<ApiKeyDto>> GetApiKeyByIdAsync(Guid id);
    Task<ServiceResult<ApiKeyDto>> UpdateApiKeyAsync(Guid id, UpdateApiKeyRequest request);
    Task<ServiceResult<bool>> DeleteApiKeyAsync(Guid id);
    Task<ServiceResult<PagedResult<ApiKeyDto>>> ListApiKeysAsync(int page, int pageSize);
    Task<ServiceResult<ApiKeyValidationResult>> ValidateApiKeyAsync(string apiKey);
}

public class ApiKeyValidationResult
{
    public bool IsValid { get; set; }
    public Guid? ApiKeyId { get; set; }
    public string? Role { get; set; }
    public string? Name { get; set; }
}

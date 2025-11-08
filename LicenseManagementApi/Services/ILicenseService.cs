using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public interface ILicenseService
{
    Task<ServiceResult<LicenseDto>> CreateLicenseAsync(CreateLicenseRequest request);
    Task<ServiceResult<LicenseDto>> GetLicenseByIdAsync(Guid id);
    Task<ServiceResult<LicenseDto>> UpdateLicenseAsync(Guid id, UpdateLicenseRequest request);
    Task<ServiceResult<bool>> DeleteLicenseAsync(Guid id);
    Task<ServiceResult<bool>> RevokeLicenseAsync(Guid id);
    Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesAsync(int page, int pageSize);
    Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByCustomerAsync(Guid customerId, int page, int pageSize);
    Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByProductAsync(Guid productId, int page, int pageSize);
    Task<ServiceResult<PagedResult<LicenseDto>>> ListLicensesByStatusAsync(LicenseStatus status, int page, int pageSize);
    Task<ServiceResult<LicenseValidationResult>> ValidateLicenseKeyAsync(string licenseKey);
    Task<ServiceResult<LicenseActivationResult>> ActivateLicenseKeyAsync(string licenseKey);
}

public class LicenseValidationResult
{
    public bool IsValid { get; set; }
    public string? Reason { get; set; }
    public LicenseDto? License { get; set; }
}

public class LicenseActivationResult
{
    public bool IsActivated { get; set; }
    public string? Reason { get; set; }
    public int CurrentActivations { get; set; }
    public int MaxActivations { get; set; }
}

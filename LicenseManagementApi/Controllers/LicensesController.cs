using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicensesController : BaseApiController
{
    private readonly ILicenseService _licenseService;
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(ILicenseService licenseService, ILogger<LicensesController> logger)
    {
        _licenseService = licenseService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new license
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseRequest request)
    {
        var result = await _licenseService.CreateLicenseAsync(request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get license by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLicenseById(Guid id)
    {
        var result = await _licenseService.GetLicenseByIdAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update license by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLicense(Guid id, [FromBody] UpdateLicenseRequest request)
    {
        var result = await _licenseService.UpdateLicenseAsync(id, request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete license by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLicense(Guid id)
    {
        var result = await _licenseService.DeleteLicenseAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Revoke a license by ID
    /// </summary>
    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> RevokeLicense(Guid id)
    {
        var result = await _licenseService.RevokeLicenseAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// List all licenses with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListLicenses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _licenseService.ListLicensesAsync(page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// List licenses by customer ID
    /// </summary>
    [HttpGet("by-customer/{customerId}")]
    public async Task<IActionResult> ListLicensesByCustomer(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _licenseService.ListLicensesByCustomerAsync(customerId, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// List licenses by product ID
    /// </summary>
    [HttpGet("by-product/{productId}")]
    public async Task<IActionResult> ListLicensesByProduct(
        Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _licenseService.ListLicensesByProductAsync(productId, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// List licenses by status
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> ListLicensesByStatus(
        LicenseStatus status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _licenseService.ListLicensesByStatusAsync(status, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Validate a license key
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateLicenseKey([FromBody] ValidateLicenseKeyRequest request)
    {
        var result = await _licenseService.ValidateLicenseKeyAsync(request.LicenseKey);
        return ToActionResult(result);
    }

    /// <summary>
    /// Activate a license key
    /// </summary>
    [HttpPost("activate")]
    public async Task<IActionResult> ActivateLicenseKey([FromBody] ActivateLicenseKeyRequest request)
    {
        var result = await _licenseService.ActivateLicenseKeyAsync(request.LicenseKey);
        return ToActionResult(result);
    }
}

public class ValidateLicenseKeyRequest
{
    public string LicenseKey { get; set; } = string.Empty;
}

public class ActivateLicenseKeyRequest
{
    public string LicenseKey { get; set; } = string.Empty;
}

using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RsaKeysController : BaseApiController
{
    private readonly IRsaKeyService _rsaKeyService;
    private readonly ILogger<RsaKeysController> _logger;

    public RsaKeysController(IRsaKeyService rsaKeyService, ILogger<RsaKeysController> logger)
    {
        _rsaKeyService = rsaKeyService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a new RSA key pair
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GenerateKeyPair([FromBody] GenerateRsaKeyRequest request)
    {
        var result = await _rsaKeyService.GenerateKeyPairAsync(request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get RSA key by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetKeyById(Guid id)
    {
        var result = await _rsaKeyService.GetKeyByIdAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Download private key for RSA key pair
    /// </summary>
    [HttpGet("{id}/private-key")]
    public async Task<IActionResult> DownloadPrivateKey(Guid id)
    {
        var result = await _rsaKeyService.DownloadPrivateKeyAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// List all RSA keys with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListKeys(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _rsaKeyService.ListKeysAsync(page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update RSA key metadata by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateKey(Guid id, [FromBody] UpdateRsaKeyRequest request)
    {
        var result = await _rsaKeyService.UpdateKeyAsync(id, request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete RSA key by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKey(Guid id)
    {
        var result = await _rsaKeyService.DeleteKeyAsync(id);
        return ToActionResult(result);
    }
}

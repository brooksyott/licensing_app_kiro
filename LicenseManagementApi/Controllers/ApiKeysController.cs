using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : BaseApiController
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogger<ApiKeysController> _logger;

    public ApiKeysController(IApiKeyService apiKeyService, ILogger<ApiKeysController> logger)
    {
        _apiKeyService = apiKeyService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new API key
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyRequest request)
    {
        var result = await _apiKeyService.CreateApiKeyAsync(request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get API key by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApiKeyById(Guid id)
    {
        var result = await _apiKeyService.GetApiKeyByIdAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// List all API keys with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListApiKeys(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _apiKeyService.ListApiKeysAsync(page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update API key by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApiKey(Guid id, [FromBody] UpdateApiKeyRequest request)
    {
        var result = await _apiKeyService.UpdateApiKeyAsync(id, request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete API key by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApiKey(Guid id)
    {
        var result = await _apiKeyService.DeleteApiKeyAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Validate an API key
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateApiKey([FromBody] ValidateApiKeyRequest request)
    {
        var result = await _apiKeyService.ValidateApiKeyAsync(request.ApiKey);
        return ToActionResult(result);
    }
}

public class ValidateApiKeyRequest
{
    public string ApiKey { get; set; } = string.Empty;
}

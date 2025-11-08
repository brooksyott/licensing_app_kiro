using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkusController : BaseApiController
{
    private readonly ISkuService _skuService;
    private readonly ILogger<SkusController> _logger;

    public SkusController(ISkuService skuService, ILogger<SkusController> logger)
    {
        _skuService = skuService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new SKU
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSku([FromBody] CreateSkuRequest request)
    {
        var result = await _skuService.CreateSkuAsync(request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get SKU by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSkuById(Guid id)
    {
        var result = await _skuService.GetSkuByIdAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// List all SKUs with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListSkus(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _skuService.ListSkusAsync(page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// List SKUs by product ID with pagination
    /// </summary>
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> ListSkusByProduct(
        Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _skuService.ListSkusByProductAsync(productId, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Search SKUs by term
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchSkus(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _skuService.SearchSkusAsync(searchTerm, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update SKU by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSku(Guid id, [FromBody] UpdateSkuRequest request)
    {
        var result = await _skuService.UpdateSkuAsync(id, request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete SKU by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSku(Guid id)
    {
        var result = await _skuService.DeleteSkuAsync(id);
        return ToActionResult(result);
    }
}

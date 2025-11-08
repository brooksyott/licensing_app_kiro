using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models.Requests;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : BaseApiController
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        var result = await _customerService.CreateCustomerAsync(request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var result = await _customerService.GetCustomerByIdAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get customer by name
    /// </summary>
    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetCustomerByName(string name)
    {
        var result = await _customerService.GetCustomerByNameAsync(name);
        return ToActionResult(result);
    }

    /// <summary>
    /// Search customers by term
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _customerService.SearchCustomersAsync(searchTerm, page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// List all customers with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _customerService.ListCustomersAsync(page, pageSize);
        return ToActionResult(result);
    }

    /// <summary>
    /// Update customer by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var result = await _customerService.UpdateCustomerAsync(id, request);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete customer by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete customer by name
    /// </summary>
    [HttpDelete("by-name/{name}")]
    public async Task<IActionResult> DeleteCustomerByName(string name)
    {
        var result = await _customerService.DeleteCustomerByNameAsync(name);
        return ToActionResult(result);
    }
}

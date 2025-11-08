using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class CreateSkuRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU code is required")]
    [StringLength(100, ErrorMessage = "SKU code cannot exceed 100 characters")]
    public string SkuCode { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
}

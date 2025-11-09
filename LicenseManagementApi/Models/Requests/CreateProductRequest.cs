using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product code is required")]
    [StringLength(100, ErrorMessage = "Product code cannot exceed 100 characters")]
    public string ProductCode { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
}

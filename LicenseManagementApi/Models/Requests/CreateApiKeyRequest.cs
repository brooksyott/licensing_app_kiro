using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class CreateApiKeyRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
    public string Role { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "CreatedBy cannot exceed 200 characters")]
    public string CreatedBy { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

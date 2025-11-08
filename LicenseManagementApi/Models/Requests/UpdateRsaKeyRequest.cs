using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class UpdateRsaKeyRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}

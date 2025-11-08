using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class GenerateRsaKeyRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(2048, 4096, ErrorMessage = "Key size must be between 2048 and 4096 bits")]
    public int KeySize { get; set; } = 2048;

    [Required(ErrorMessage = "CreatedBy is required")]
    [StringLength(200, ErrorMessage = "CreatedBy cannot exceed 200 characters")]
    public string CreatedBy { get; set; } = string.Empty;
}

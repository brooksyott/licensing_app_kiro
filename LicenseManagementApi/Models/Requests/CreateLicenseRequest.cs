using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class CreateLicenseRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one SKU must be selected")]
    public List<Guid> SkuIds { get; set; } = new();

    [Required]
    public Guid RsaKeyId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LicenseType { get; set; } = string.Empty;

    public DateTime? ExpirationDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxActivations { get; set; } = 1;
}

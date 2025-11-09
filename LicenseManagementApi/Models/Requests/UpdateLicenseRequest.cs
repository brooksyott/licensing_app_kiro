using System.ComponentModel.DataAnnotations;

namespace LicenseManagementApi.Models.Requests;

public class UpdateLicenseRequest
{
    [MinLength(1, ErrorMessage = "At least one SKU must be selected")]
    public List<Guid>? SkuIds { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? LicenseType { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxActivations { get; set; }
}

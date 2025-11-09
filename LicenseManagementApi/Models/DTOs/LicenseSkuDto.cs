namespace LicenseManagementApi.Models.DTOs;

public class LicenseSkuDto
{
    public Guid SkuId { get; set; }
    public string SkuName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
}

using LicenseManagementApi.Models.Entities;

namespace LicenseManagementApi.Models.DTOs;

public class LicenseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public List<LicenseSkuDto> Skus { get; set; } = new();
    public Guid RsaKeyId { get; set; }
    public string RsaKeyName { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public string SignedPayload { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
    public int CurrentActivations { get; set; }
    public LicenseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

namespace LicenseManagementApi.Models.Entities;

public class Sku
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<License> Licenses { get; set; } = new List<License>();
}

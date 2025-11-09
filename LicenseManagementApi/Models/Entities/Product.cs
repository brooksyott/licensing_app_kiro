namespace LicenseManagementApi.Models.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Sku> Skus { get; set; } = new List<Sku>();
    public ICollection<License> Licenses { get; set; } = new List<License>();
}

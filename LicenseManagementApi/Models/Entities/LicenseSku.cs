namespace LicenseManagementApi.Models.Entities;

public class LicenseSku
{
    public Guid LicenseId { get; set; }
    public License License { get; set; } = null!;
    
    public Guid SkuId { get; set; }
    public Sku Sku { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}

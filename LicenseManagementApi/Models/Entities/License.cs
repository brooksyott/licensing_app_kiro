namespace LicenseManagementApi.Models.Entities;

public class License
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid RsaKeyId { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string LicenseKeyHash { get; set; } = string.Empty;
    public string SignedPayload { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
    public int CurrentActivations { get; set; }
    public LicenseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public RsaKey RsaKey { get; set; } = null!;
    
    // Many-to-many relationship with SKUs
    public ICollection<LicenseSku> LicenseSkus { get; set; } = new List<LicenseSku>();
}

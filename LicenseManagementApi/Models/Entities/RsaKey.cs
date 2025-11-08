namespace LicenseManagementApi.Models.Entities;

public class RsaKey
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKeyEncrypted { get; set; } = string.Empty;
    public int KeySize { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<License> Licenses { get; set; } = new List<License>();
}

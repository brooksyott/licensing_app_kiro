using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Models.Entities;

namespace LicenseManagementApi.Data;

public class LicenseManagementDbContext : DbContext
{
    public LicenseManagementDbContext(DbContextOptions<LicenseManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sku> Skus { get; set; }
    public DbSet<RsaKey> RsaKeys { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Customer customer)
            {
                if (entry.State == EntityState.Added)
                    customer.CreatedAt = DateTime.UtcNow;
                customer.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Product product)
            {
                if (entry.State == EntityState.Added)
                    product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Sku sku)
            {
                if (entry.State == EntityState.Added)
                    sku.CreatedAt = DateTime.UtcNow;
                sku.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is RsaKey rsaKey)
            {
                if (entry.State == EntityState.Added)
                    rsaKey.CreatedAt = DateTime.UtcNow;
                rsaKey.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is License license)
            {
                if (entry.State == EntityState.Added)
                    license.CreatedAt = DateTime.UtcNow;
                license.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is ApiKey apiKey)
            {
                if (entry.State == EntityState.Added)
                    apiKey.CreatedAt = DateTime.UtcNow;
                apiKey.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Name); // Performance index for search queries
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductCode).IsUnique();
            entity.HasIndex(e => e.Name); // Performance index for search queries
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.ProductCode).IsRequired();
            entity.Property(e => e.Version).IsRequired();
        });

        // SKU configuration
        modelBuilder.Entity<Sku>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SkuCode).IsUnique();
            entity.HasIndex(e => e.ProductId); // Performance index for filtering by product
            entity.HasIndex(e => e.Name); // Performance index for search queries
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.SkuCode).IsRequired();
            
            // Relationship with Product - cascade delete
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Skus)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RsaKey configuration
        modelBuilder.Entity<RsaKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.PublicKey).IsRequired();
            entity.Property(e => e.PrivateKeyEncrypted).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
        });

        // License configuration
        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LicenseKeyHash).IsUnique();
            entity.HasIndex(e => e.CustomerId); // Performance index for filtering by customer
            entity.HasIndex(e => e.ProductId); // Performance index for filtering by product
            entity.HasIndex(e => e.Status); // Performance index for filtering by status
            entity.Property(e => e.LicenseKey).IsRequired();
            entity.Property(e => e.LicenseKeyHash).IsRequired();
            entity.Property(e => e.SignedPayload).IsRequired();
            entity.Property(e => e.LicenseType).IsRequired();
            
            // Relationship with Customer - restrict delete
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Licenses)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship with Product - restrict delete
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Licenses)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship with SKU - set null on delete
            entity.HasOne(e => e.Sku)
                .WithMany(s => s.Licenses)
                .HasForeignKey(e => e.SkuId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relationship with RsaKey - restrict delete
            entity.HasOne(e => e.RsaKey)
                .WithMany(r => r.Licenses)
                .HasForeignKey(e => e.RsaKeyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ApiKey configuration
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.KeyHash).IsUnique();
            entity.HasIndex(e => e.IsActive); // Performance index for filtering active keys
            entity.Property(e => e.KeyHash).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
        });
    }
}

# Design Document

## Overview

This design document outlines the technical approach for implementing multiple SKUs per license in the License Management System. The solution involves creating a many-to-many relationship between licenses and SKUs through a junction table, updating the API endpoints to handle SKU arrays, and modifying the UI to support multi-select SKU selection.

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend (Vue)                       │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  LicenseForm Component (Multi-Select SKU Interface)    │ │
│  └────────────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  LicenseTable Component (Display Multiple SKUs)        │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTP/JSON
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Backend API (.NET)                      │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  LicensesController (Updated Endpoints)                │ │
│  └────────────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  LicenseService (Business Logic for SKU Management)    │ │
│  └────────────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  DTOs (Updated to include SKU arrays)                  │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ EF Core
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Database (PostgreSQL)                     │
│  ┌──────────────┐    ┌──────────────────┐    ┌───────────┐ │
│  │   Licenses   │────│  LicenseSkus     │────│   Skus    │ │
│  │              │    │  (Junction)      │    │           │ │
│  └──────────────┘    └──────────────────┘    └───────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### Database Schema Changes

#### New Junction Table: LicenseSkus

```sql
CREATE TABLE LicenseSkus (
    LicenseId UUID NOT NULL,
    SkuId UUID NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (LicenseId, SkuId),
    FOREIGN KEY (LicenseId) REFERENCES Licenses(Id) ON DELETE CASCADE,
    FOREIGN KEY (SkuId) REFERENCES Skus(Id) ON DELETE CASCADE
);

CREATE INDEX IX_LicenseSkus_LicenseId ON LicenseSkus(LicenseId);
CREATE INDEX IX_LicenseSkus_SkuId ON LicenseSkus(SkuId);
```

#### Updated License Table

Remove the single `SkuId` foreign key column from the Licenses table:

```sql
ALTER TABLE Licenses DROP COLUMN SkuId;
ALTER TABLE Licenses DROP COLUMN SkuName;
```

### Backend Components

#### Entity Models

**LicenseSku.cs** (New Junction Entity)
```csharp
public class LicenseSku
{
    public Guid LicenseId { get; set; }
    public License License { get; set; } = null!;
    
    public Guid SkuId { get; set; }
    public Sku Sku { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}
```

**License.cs** (Updated)
```csharp
public class License
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid RsaKeyId { get; set; }
    public RsaKey RsaKey { get; set; } = null!;
    
    // Many-to-many relationship with SKUs
    public ICollection<LicenseSku> LicenseSkus { get; set; } = new List<LicenseSku>();
    
    public string LicenseType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
    public int CurrentActivations { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### DTOs

**LicenseDto.cs** (Updated)
```csharp
public class LicenseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    
    // Changed from single SKU to array of SKUs
    public List<LicenseSkuDto> Skus { get; set; } = new();
    
    public Guid RsaKeyId { get; set; }
    public string RsaKeyName { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
    public int CurrentActivations { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LicenseSkuDto
{
    public Guid SkuId { get; set; }
    public string SkuName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
}
```

**CreateLicenseDto.cs** (Updated)
```csharp
public class CreateLicenseDto
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    
    // Changed from single SkuId to array of SkuIds
    public List<Guid> SkuIds { get; set; } = new();
    
    public Guid RsaKeyId { get; set; }
    public string LicenseType { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
}
```

**UpdateLicenseDto.cs** (Updated)
```csharp
public class UpdateLicenseDto
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    
    // Changed from single SkuId to array of SkuIds
    public List<Guid> SkuIds { get; set; } = new();
    
    public Guid RsaKeyId { get; set; }
    public string LicenseType { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int MaxActivations { get; set; }
}
```

#### Service Layer

**ILicenseService.cs** (Interface remains similar, implementation changes)

**LicenseService.cs** (Key Methods)

```csharp
public async Task<ServiceResult<LicenseDto>> CreateAsync(CreateLicenseDto dto)
{
    // Validate SKU IDs
    if (dto.SkuIds == null || !dto.SkuIds.Any())
    {
        return ServiceResult<LicenseDto>.Failure("At least one SKU must be selected");
    }
    
    // Remove duplicates
    var uniqueSkuIds = dto.SkuIds.Distinct().ToList();
    
    // Verify all SKUs exist
    var skus = await _context.Skus
        .Where(s => uniqueSkuIds.Contains(s.Id))
        .ToListAsync();
    
    if (skus.Count != uniqueSkuIds.Count)
    {
        var invalidIds = uniqueSkuIds.Except(skus.Select(s => s.Id));
        return ServiceResult<LicenseDto>.Failure($"Invalid SKU IDs: {string.Join(", ", invalidIds)}");
    }
    
    // Create license
    var license = new License
    {
        Id = Guid.NewGuid(),
        CustomerId = dto.CustomerId,
        ProductId = dto.ProductId,
        RsaKeyId = dto.RsaKeyId,
        LicenseType = dto.LicenseType,
        ExpirationDate = dto.ExpirationDate,
        MaxActivations = dto.MaxActivations,
        Status = "Active",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    
    // Generate license key
    license.LicenseKey = await _licenseKeyGenerator.GenerateAsync(license, skus);
    
    // Create license-SKU associations
    foreach (var skuId in uniqueSkuIds)
    {
        license.LicenseSkus.Add(new LicenseSku
        {
            LicenseId = license.Id,
            SkuId = skuId,
            CreatedAt = DateTime.UtcNow
        });
    }
    
    _context.Licenses.Add(license);
    await _context.SaveChangesAsync();
    
    return ServiceResult<LicenseDto>.Success(await MapToDto(license));
}

public async Task<ServiceResult<LicenseDto>> UpdateAsync(Guid id, UpdateLicenseDto dto)
{
    var license = await _context.Licenses
        .Include(l => l.LicenseSkus)
        .FirstOrDefaultAsync(l => l.Id == id);
    
    if (license == null)
    {
        return ServiceResult<LicenseDto>.Failure("License not found");
    }
    
    // Validate SKU IDs
    if (dto.SkuIds == null || !dto.SkuIds.Any())
    {
        return ServiceResult<LicenseDto>.Failure("At least one SKU must be selected");
    }
    
    var uniqueSkuIds = dto.SkuIds.Distinct().ToList();
    
    // Verify all SKUs exist
    var skus = await _context.Skus
        .Where(s => uniqueSkuIds.Contains(s.Id))
        .ToListAsync();
    
    if (skus.Count != uniqueSkuIds.Count)
    {
        var invalidIds = uniqueSkuIds.Except(skus.Select(s => s.Id));
        return ServiceResult<LicenseDto>.Failure($"Invalid SKU IDs: {string.Join(", ", invalidIds)}");
    }
    
    // Update license properties
    license.CustomerId = dto.CustomerId;
    license.ProductId = dto.ProductId;
    license.RsaKeyId = dto.RsaKeyId;
    license.LicenseType = dto.LicenseType;
    license.ExpirationDate = dto.ExpirationDate;
    license.MaxActivations = dto.MaxActivations;
    license.UpdatedAt = DateTime.UtcNow;
    
    // Update SKU associations
    // Remove existing associations
    _context.LicenseSkus.RemoveRange(license.LicenseSkus);
    
    // Add new associations
    license.LicenseSkus.Clear();
    foreach (var skuId in uniqueSkuIds)
    {
        license.LicenseSkus.Add(new LicenseSku
        {
            LicenseId = license.Id,
            SkuId = skuId,
            CreatedAt = DateTime.UtcNow
        });
    }
    
    await _context.SaveChangesAsync();
    
    return ServiceResult<LicenseDto>.Success(await MapToDto(license));
}

private async Task<LicenseDto> MapToDto(License license)
{
    var skuDetails = await _context.LicenseSkus
        .Where(ls => ls.LicenseId == license.Id)
        .Include(ls => ls.Sku)
        .Select(ls => new LicenseSkuDto
        {
            SkuId = ls.SkuId,
            SkuName = ls.Sku.Name,
            SkuCode = ls.Sku.SkuCode
        })
        .ToListAsync();
    
    return new LicenseDto
    {
        Id = license.Id,
        CustomerId = license.CustomerId,
        CustomerName = license.Customer.Name,
        ProductId = license.ProductId,
        ProductName = license.Product.Name,
        Skus = skuDetails,
        RsaKeyId = license.RsaKeyId,
        RsaKeyName = license.RsaKey.Name,
        LicenseType = license.LicenseType,
        Status = license.Status,
        ExpirationDate = license.ExpirationDate,
        MaxActivations = license.MaxActivations,
        CurrentActivations = license.CurrentActivations,
        LicenseKey = license.LicenseKey,
        CreatedAt = license.CreatedAt,
        UpdatedAt = license.UpdatedAt
    };
}
```

### Frontend Components

#### TypeScript Types

**license.ts** (Updated)
```typescript
export interface License {
  id: string
  customerId: string
  customerName: string
  productId: string
  productName: string
  
  // Changed from single SKU to array of SKUs
  skus: LicenseSku[]
  
  rsaKeyId: string
  rsaKeyName: string
  licenseType: string
  status: string
  expirationDate: string | null
  maxActivations: number
  currentActivations: number
  licenseKey: string
  createdAt: string
  updatedAt: string
}

export interface LicenseSku {
  skuId: string
  skuName: string
  skuCode: string
}

export interface CreateLicenseDto {
  customerId: string
  productId: string
  
  // Changed from single skuId to array of skuIds
  skuIds: string[]
  
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}

export interface UpdateLicenseDto {
  customerId: string
  productId: string
  
  // Changed from single skuId to array of skuIds
  skuIds: string[]
  
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}
```

#### Vue Components

**LicenseForm.vue** (Updated)

Key changes:
- Replace single SKU dropdown with multi-select component
- Add visual feedback for selected SKUs
- Group SKUs by product for better UX
- Validate that at least one SKU is selected

```vue
<template>
  <div class="form-group">
    <label for="skus">SKUs *</label>
    <div class="multi-select-container">
      <div 
        v-for="sku in availableSkus" 
        :key="sku.id"
        class="sku-checkbox"
      >
        <input
          type="checkbox"
          :id="`sku-${sku.id}`"
          :value="sku.id"
          v-model="formData.skuIds"
        />
        <label :for="`sku-${sku.id}`">
          {{ sku.name }} ({{ sku.skuCode }}) - {{ sku.productName }}
        </label>
      </div>
    </div>
    <span v-if="errors.skuIds" class="error">{{ errors.skuIds }}</span>
  </div>
</template>
```

**LicenseTable.vue** (Updated)

Key changes:
- Display multiple SKUs in a formatted list
- Show SKU count badge
- Truncate long SKU lists with "show more" functionality

```vue
<template>
  <td>
    <div class="sku-list">
      <span 
        v-for="(sku, index) in license.skus.slice(0, 2)" 
        :key="sku.skuId"
        class="sku-badge"
      >
        {{ sku.skuName }}
      </span>
      <span 
        v-if="license.skus.length > 2" 
        class="sku-count"
      >
        +{{ license.skus.length - 2 }} more
      </span>
    </div>
  </td>
</template>
```

## Data Models

### Entity Relationship Diagram

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│  Licenses   │         │ LicenseSkus  │         │    Skus     │
├─────────────┤         ├──────────────┤         ├─────────────┤
│ Id (PK)     │────────<│ LicenseId(FK)│>────────│ Id (PK)     │
│ CustomerId  │         │ SkuId (FK)   │         │ ProductId   │
│ ProductId   │         │ CreatedAt    │         │ Name        │
│ RsaKeyId    │         └──────────────┘         │ SkuCode     │
│ LicenseType │                                   │ Description │
│ Status      │                                   └─────────────┘
│ ...         │
└─────────────┘
```

## Error Handling

### Validation Errors

1. **No SKUs Selected**: Return 400 Bad Request with message "At least one SKU must be selected"
2. **Invalid SKU IDs**: Return 400 Bad Request with message listing invalid SKU IDs
3. **SKU Not Found**: Return 404 Not Found with message "One or more SKUs not found"
4. **Duplicate SKUs**: Automatically remove duplicates, no error returned

### Database Errors

1. **Foreign Key Violation**: Return 400 Bad Request with user-friendly message
2. **Concurrent Update**: Return 409 Conflict with message to retry
3. **Database Connection**: Return 503 Service Unavailable

## Testing Strategy

### Backend Tests

1. **Unit Tests**
   - Test SKU validation logic
   - Test duplicate removal
   - Test license-SKU association creation
   - Test license-SKU association updates

2. **Integration Tests**
   - Test creating license with multiple SKUs
   - Test updating license SKUs
   - Test deleting license removes all SKU associations
   - Test querying licenses with SKU data

### Frontend Tests

1. **Component Tests**
   - Test multi-select SKU interface
   - Test SKU selection/deselection
   - Test form validation with no SKUs
   - Test SKU display in table

2. **E2E Tests**
   - Test complete license creation flow with multiple SKUs
   - Test license update flow changing SKUs
   - Test license display with multiple SKUs

## Migration Strategy

### Database Migration

1. Create `LicenseSkus` junction table
2. Drop `SkuId` and `SkuName` columns from `Licenses` table
3. No data migration needed (backward compatibility not required)

### API Migration

1. Update DTOs to use SKU arrays
2. Update service layer to handle SKU arrays
3. Update controllers to accept/return new DTOs
4. Update API documentation

### Frontend Migration

1. Update TypeScript types
2. Update LicenseForm component
3. Update LicenseTable component
4. Update license service calls

## Performance Considerations

1. **Database Queries**: Use eager loading with `.Include()` to fetch SKUs with licenses
2. **Indexing**: Add indexes on `LicenseSkus` table for efficient lookups
3. **Pagination**: Ensure SKU data doesn't significantly impact pagination performance
4. **Caching**: Consider caching SKU data for frequently accessed licenses

## License Key JWT Structure

### JWT Payload Design

The license key JWT should include comprehensive information about all products and SKUs included in the license.

**JWT Claims Structure:**

```json
{
  "iss": "LicenseManagementSystem",
  "sub": "customer-guid",
  "iat": 1699564800,
  "exp": 1731100800,
  "license": {
    "id": "license-guid",
    "name": "Enterprise License - Acme Corp",
    "type": "Enterprise",
    "status": "Active",
    "maxActivations": 10,
    "products": [
      {
        "productId": "product-guid-1",
        "productName": "Product A",
        "productCode": "PROD-A",
        "skus": [
          {
            "skuId": "sku-guid-1",
            "skuName": "Product A - Enterprise Edition",
            "skuCode": "PROD-A-ENT"
          },
          {
            "skuId": "sku-guid-2",
            "skuName": "Product A - Premium Support",
            "skuCode": "PROD-A-SUPPORT"
          }
        ]
      },
      {
        "productId": "product-guid-2",
        "productName": "Product B",
        "productCode": "PROD-B",
        "skus": [
          {
            "skuId": "sku-guid-3",
            "skuName": "Product B - Standard Edition",
            "skuCode": "PROD-B-STD"
          }
        ]
      }
    ]
  }
}
```

### License Key Generator Updates

**ILicenseKeyGenerator.cs** (Updated Interface)

```csharp
public interface ILicenseKeyGenerator
{
    Task<string> GenerateAsync(License license, List<Sku> skus);
}
```

**LicenseKeyGenerator.cs** (Updated Implementation)

```csharp
public async Task<string> GenerateAsync(License license, List<Sku> skus)
{
    // Group SKUs by product
    var productGroups = skus
        .GroupBy(s => s.ProductId)
        .Select(g => new
        {
            ProductId = g.Key,
            Product = g.First().Product,
            Skus = g.ToList()
        })
        .ToList();
    
    // Build license name
    var licenseName = $"{license.LicenseType} License - {license.Customer.Name}";
    
    // Build products array for JWT
    var productsPayload = productGroups.Select(pg => new
    {
        productId = pg.ProductId.ToString(),
        productName = pg.Product.Name,
        productCode = pg.Product.ProductCode,
        skus = pg.Skus.Select(s => new
        {
            skuId = s.Id.ToString(),
            skuName = s.Name,
            skuCode = s.SkuCode
        }).ToList()
    }).ToList();
    
    // Create JWT payload
    var payload = new Dictionary<string, object>
    {
        { "iss", "LicenseManagementSystem" },
        { "sub", license.CustomerId.ToString() },
        { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
        { "exp", license.ExpirationDate?.ToUnixTimeSeconds() ?? 0 },
        { "license", new
            {
                id = license.Id.ToString(),
                name = licenseName,
                type = license.LicenseType,
                status = license.Status,
                maxActivations = license.MaxActivations,
                products = productsPayload
            }
        }
    };
    
    // Sign and encode JWT
    var token = JWT.Encode(payload, _rsaKey, JwsAlgorithm.RS256);
    
    return token;
}
```

### JWT Validation

When validating a license key, applications can:

1. Decode the JWT
2. Access the `license.products` array
3. Check if specific product/SKU combinations are present
4. Verify the license status and expiration

**Example Validation Logic:**

```csharp
public bool HasAccessToProduct(string licenseKey, string productCode)
{
    var payload = JWT.Decode<Dictionary<string, object>>(licenseKey, _publicKey);
    var license = payload["license"] as Dictionary<string, object>;
    var products = license["products"] as List<object>;
    
    return products.Any(p => 
    {
        var product = p as Dictionary<string, object>;
        return product["productCode"].ToString() == productCode;
    });
}

public bool HasAccessToSku(string licenseKey, string skuCode)
{
    var payload = JWT.Decode<Dictionary<string, object>>(licenseKey, _publicKey);
    var license = payload["license"] as Dictionary<string, object>;
    var products = license["products"] as List<object>;
    
    return products.Any(p => 
    {
        var product = p as Dictionary<string, object>;
        var skus = product["skus"] as List<object>;
        return skus.Any(s => 
        {
            var sku = s as Dictionary<string, object>;
            return sku["skuCode"].ToString() == skuCode;
        });
    });
}
```

## Security Considerations

1. **Authorization**: Ensure users have permission to access SKU data
2. **Input Validation**: Validate all SKU IDs before processing
3. **SQL Injection**: Use parameterized queries (handled by EF Core)
4. **Data Integrity**: Use transactions when updating license-SKU associations
5. **JWT Security**: 
   - Sign JWTs with RSA private key
   - Include expiration time in JWT
   - Validate JWT signature on license verification
   - Include all product/SKU data to prevent tampering

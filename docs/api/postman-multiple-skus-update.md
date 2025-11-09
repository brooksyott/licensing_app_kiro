# Postman Collection Update - Multiple SKUs per License

## Overview

The Postman collection has been updated to support the new multiple SKUs per license feature. This document outlines the changes made and how to use the updated collection.

## Changes Made

### 1. Collection Variables

Added new collection variables to support multiple SKU IDs:
- `skuId2` - Second SKU ID for testing
- `skuId3` - Third SKU ID for testing

### 2. SKU Endpoints

Added new requests to create multiple SKUs for testing:
- **Create SKU 2** - Creates an "Enterprise Edition" SKU
- **Create SKU 3** - Creates a "Premium Support" SKU

These requests automatically save the created SKU IDs to the collection variables `skuId2` and `skuId3`.

### 3. License Creation

#### Updated: Create License
- Changed `skuId` field to `skuIds` array
- Now accepts an array of SKU IDs: `["{{skuId}}"]`
- Updated description to clarify array usage

#### New: Create License with Multiple SKUs
- Demonstrates creating a license with 3 SKUs
- Uses all three SKU variables: `skuId`, `skuId2`, `skuId3`
- Includes test to verify multiple SKUs in response
- Example shows Enterprise license type with 10 max activations

### 4. License Updates

#### Updated: Update License
- Kept existing request for updating license properties only
- Added description clarifying all fields are optional

#### New: Update License SKUs
- Demonstrates updating SKU associations
- Replaces all existing SKUs with new ones
- Uses 2 SKUs in the example

#### New: Update License - Add SKUs
- Shows how to update SKUs along with other properties
- Demonstrates adding a third SKU while updating license type and max activations

### 5. License Retrieval

#### Updated: Get License by ID
- Added tests to verify SKUs array in response
- Validates each SKU has required fields: `skuId`, `skuName`, `skuCode`
- Updated description to mention SKU details in response

## Testing Workflow

### Setup Phase

1. **Create Customer**
   ```
   POST /api/customers
   ```
   Saves `customerId` to collection variable

2. **Create Product**
   ```
   POST /api/products
   ```
   Saves `productId` to collection variable

3. **Create SKU 1**
   ```
   POST /api/skus
   ```
   Saves `skuId` to collection variable

4. **Create SKU 2**
   ```
   POST /api/skus
   ```
   Saves `skuId2` to collection variable

5. **Create SKU 3**
   ```
   POST /api/skus
   ```
   Saves `skuId3` to collection variable

6. **Generate RSA Key Pair**
   ```
   POST /api/rsakeys
   ```
   Saves `rsaKeyId` to collection variable

### Testing Multiple SKUs

7. **Create License with Multiple SKUs**
   ```
   POST /api/licenses
   Body: {
     "customerId": "{{customerId}}",
     "productId": "{{productId}}",
     "skuIds": ["{{skuId}}", "{{skuId2}}", "{{skuId3}}"],
     "rsaKeyId": "{{rsaKeyId}}",
     "licenseType": "Enterprise",
     "maxActivations": 10
   }
   ```
   Saves `licenseId` and verifies multiple SKUs in response

8. **Get License by ID**
   ```
   GET /api/licenses/{{licenseId}}
   ```
   Verifies SKUs array with all details

9. **Update License SKUs**
   ```
   PUT /api/licenses/{{licenseId}}
   Body: {
     "skuIds": ["{{skuId}}", "{{skuId2}}"]
   }
   ```
   Removes one SKU, keeps two

10. **Update License - Add SKUs**
    ```
    PUT /api/licenses/{{licenseId}}
    Body: {
      "skuIds": ["{{skuId}}", "{{skuId2}}", "{{skuId3}}"],
      "licenseType": "Enterprise",
      "maxActivations": 15
    }
    ```
    Adds SKU back and updates other properties

## API Request/Response Examples

### Create License with Multiple SKUs

**Request:**
```json
{
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "productId": "223e4567-e89b-12d3-a456-426614174000",
  "skuIds": [
    "323e4567-e89b-12d3-a456-426614174000",
    "423e4567-e89b-12d3-a456-426614174000",
    "523e4567-e89b-12d3-a456-426614174000"
  ],
  "rsaKeyId": "623e4567-e89b-12d3-a456-426614174000",
  "licenseType": "Enterprise",
  "expirationDate": "2025-12-31T23:59:59Z",
  "maxActivations": 10
}
```

**Response:**
```json
{
  "id": "723e4567-e89b-12d3-a456-426614174000",
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "customerName": "Acme Corporation",
  "productId": "223e4567-e89b-12d3-a456-426614174000",
  "productName": "Enterprise Suite",
  "skus": [
    {
      "skuId": "323e4567-e89b-12d3-a456-426614174000",
      "skuName": "Professional Edition",
      "skuCode": "ENT-001-PRO"
    },
    {
      "skuId": "423e4567-e89b-12d3-a456-426614174000",
      "skuName": "Enterprise Edition",
      "skuCode": "ENT-001-ENT"
    },
    {
      "skuId": "523e4567-e89b-12d3-a456-426614174000",
      "skuName": "Premium Support",
      "skuCode": "ENT-001-SUP"
    }
  ],
  "rsaKeyId": "623e4567-e89b-12d3-a456-426614174000",
  "rsaKeyName": "Production Key 2024",
  "licenseKey": "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XX",
  "signedPayload": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "licenseType": "Enterprise",
  "expirationDate": "2025-12-31T23:59:59Z",
  "maxActivations": 10,
  "currentActivations": 0,
  "status": "Active",
  "createdAt": "2024-11-08T10:00:00Z",
  "updatedAt": "2024-11-08T10:00:00Z"
}
```

### Update License SKUs

**Request:**
```json
{
  "skuIds": [
    "323e4567-e89b-12d3-a456-426614174000",
    "423e4567-e89b-12d3-a456-426614174000"
  ]
}
```

**Response:**
```json
{
  "id": "723e4567-e89b-12d3-a456-426614174000",
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "customerName": "Acme Corporation",
  "productId": "223e4567-e89b-12d3-a456-426614174000",
  "productName": "Enterprise Suite",
  "skus": [
    {
      "skuId": "323e4567-e89b-12d3-a456-426614174000",
      "skuName": "Professional Edition",
      "skuCode": "ENT-001-PRO"
    },
    {
      "skuId": "423e4567-e89b-12d3-a456-426614174000",
      "skuName": "Enterprise Edition",
      "skuCode": "ENT-001-ENT"
    }
  ],
  "rsaKeyId": "623e4567-e89b-12d3-a456-426614174000",
  "rsaKeyName": "Production Key 2024",
  "licenseKey": "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XX",
  "signedPayload": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "licenseType": "Enterprise",
  "expirationDate": "2025-12-31T23:59:59Z",
  "maxActivations": 10,
  "currentActivations": 0,
  "status": "Active",
  "createdAt": "2024-11-08T10:00:00Z",
  "updatedAt": "2024-11-08T10:15:00Z"
}
```

## Validation Rules

The API enforces the following validation rules for SKUs:

1. **At least one SKU required** - A license must have at least one SKU
2. **Duplicate SKU IDs are deduplicated** - If you provide duplicate SKU IDs, they will be automatically deduplicated
3. **All SKU IDs must exist** - All provided SKU IDs must exist in the database
4. **SKUs can be from different products** - A license can include SKUs from multiple products

## JWT Structure

The signed payload (JWT) now includes all SKU information grouped by product:

```json
{
  "iss": "LicenseManagementSystem",
  "sub": "123e4567-e89b-12d3-a456-426614174000",
  "iat": 1699444800,
  "exp": 1735689599,
  "license": {
    "id": "723e4567-e89b-12d3-a456-426614174000",
    "name": "Enterprise License - Acme Corporation",
    "type": "Enterprise",
    "status": "Active",
    "maxActivations": 10,
    "products": [
      {
        "productId": "223e4567-e89b-12d3-a456-426614174000",
        "productName": "Enterprise Suite",
        "productCode": "ENT-001",
        "skus": [
          {
            "skuId": "323e4567-e89b-12d3-a456-426614174000",
            "skuName": "Professional Edition",
            "skuCode": "ENT-001-PRO"
          },
          {
            "skuId": "423e4567-e89b-12d3-a456-426614174000",
            "skuName": "Enterprise Edition",
            "skuCode": "ENT-001-ENT"
          }
        ]
      }
    ]
  }
}
```

## Breaking Changes

⚠️ **Important:** The license creation and update endpoints now expect `skuIds` (array) instead of `skuId` (single value).

### Migration Guide

**Old format (deprecated):**
```json
{
  "customerId": "...",
  "productId": "...",
  "skuId": "323e4567-e89b-12d3-a456-426614174000",
  "rsaKeyId": "...",
  "licenseType": "Commercial",
  "maxActivations": 5
}
```

**New format:**
```json
{
  "customerId": "...",
  "productId": "...",
  "skuIds": ["323e4567-e89b-12d3-a456-426614174000"],
  "rsaKeyId": "...",
  "licenseType": "Commercial",
  "maxActivations": 5
}
```

## Notes

- The `productId` field in the license creation request is still required for backward compatibility, but the license can now include SKUs from multiple products
- When updating a license, providing `skuIds` will replace ALL existing SKU associations
- To keep existing SKUs and add new ones, you must include all SKU IDs (both existing and new) in the update request
- The license response always includes the full SKU details (ID, name, and code) for each associated SKU

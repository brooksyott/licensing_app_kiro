# Postman Collection Guide

## Overview

The `LicenseManagementApi.postman_collection.json` file contains a comprehensive collection of all API endpoints for the License Management API. This collection is organized by resource type and includes example requests, response validation tests, and environment variables.

## Getting Started

### 1. Import the Collection

1. Open Postman
2. Click "Import" in the top left
3. Select the `LicenseManagementApi.postman_collection.json` file
4. The collection will be imported with all endpoints organized into folders

### 2. Configure Environment Variables

The collection includes the following variables that you need to configure:

- **baseUrl**: The base URL of your API (default: `http://localhost:5000`)
- **Auth_Key**: Your API key for authentication (required for all endpoints except health check)

To set these variables:
1. Click on the collection name in Postman
2. Go to the "Variables" tab
3. Update the "Current Value" for `baseUrl` and `Auth_Key`

### 3. Create an API Key

Before you can use most endpoints, you need to create an API key:

1. First, you'll need to temporarily disable authentication or use a bootstrap key
2. Use the "Create API Key" request in the "API Keys" folder
3. Copy the returned `apiKey` value from the response
4. Set it as the `Auth_Key` collection variable
5. Save the API key securely - it won't be retrievable later

## Collection Structure

The collection is organized into the following folders:

### Health Check
- **Get Health Status**: Check API health and database connectivity (no authentication required)

### Customers
- Create Customer
- Get Customer by ID
- Get Customer by Name
- List Customers (with pagination)
- Search Customers
- Update Customer
- Delete Customer by ID
- Delete Customer by Name

### Products
- Create Product
- Get Product by ID
- List Products (with pagination)
- Search Products
- Update Product
- Delete Product

### SKUs
- Create SKU
- Get SKU by ID
- List SKUs (with pagination)
- List SKUs by Product
- Search SKUs
- Update SKU
- Delete SKU

### RSA Keys
- Generate RSA Key Pair
- Get RSA Key by ID
- Download Private Key
- List RSA Keys (with pagination)
- Update RSA Key
- Delete RSA Key

### Licenses
- Create License
- Get License by ID
- List Licenses (with pagination)
- List Licenses by Customer
- List Licenses by Product
- List Licenses by Status
- Update License
- Revoke License
- Validate License Key
- Activate License Key
- Delete License

### API Keys
- Create API Key
- Get API Key by ID
- List API Keys (with pagination)
- Update API Key
- Validate API Key
- Delete API Key

## Automated Tests

Each request includes automated tests that validate:
- Response status codes (200, 201, 400, 404, etc.)
- Response structure and required fields
- Automatic variable extraction (IDs are saved to collection variables)

## Workflow Example

Here's a typical workflow for creating and managing a license:

1. **Create an API Key** (API Keys → Create API Key)
   - Save the returned API key to the `Auth_Key` variable

2. **Create a Customer** (Customers → Create Customer)
   - The customer ID is automatically saved to `{{customerId}}`

3. **Create a Product** (Products → Create Product)
   - The product ID is automatically saved to `{{productId}}`

4. **Create a SKU** (SKUs → Create SKU)
   - Uses `{{productId}}` from the previous step
   - The SKU ID is automatically saved to `{{skuId}}`

5. **Generate RSA Key Pair** (RSA Keys → Generate RSA Key Pair)
   - The RSA key ID is automatically saved to `{{rsaKeyId}}`

6. **Create a License** (Licenses → Create License)
   - Uses `{{customerId}}`, `{{productId}}`, `{{skuId}}`, and `{{rsaKeyId}}`
   - Returns a license key that can be used for validation/activation

7. **Validate License Key** (Licenses → Validate License Key)
   - Check if a license key is valid

8. **Activate License Key** (Licenses → Activate License Key)
   - Activate the license and increment activation count

## Tips

- **Variable Chaining**: The collection automatically saves IDs from create operations, making it easy to chain requests together
- **Pagination**: All list endpoints support `page` and `pageSize` query parameters (default: page=1, pageSize=10, max=100)
- **Authentication**: All endpoints except `/health` require the `Auth_Key` header
- **Test Scripts**: Review the "Tests" tab in each request to see what validations are performed
- **Example Data**: All POST and PUT requests include realistic example data that you can modify

## Troubleshooting

### 401 Unauthorized
- Ensure your `Auth_Key` variable is set correctly
- Verify the API key is active and not expired

### 404 Not Found
- Check that the ID variables (`{{customerId}}`, `{{productId}}`, etc.) are set
- Verify the resource exists by using the corresponding "Get by ID" or "List" endpoint

### 400 Bad Request
- Review the request body for validation errors
- Check that all required fields are provided
- Ensure data types match the expected format (e.g., GUIDs for IDs, valid email format)

## Additional Resources

- API Documentation: See the main README.md for detailed API documentation
- Requirements: See `.kiro/specs/license-management-api/requirements.md`
- Design: See `.kiro/specs/license-management-api/design.md`

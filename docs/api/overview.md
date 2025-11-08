# API Overview

The License Management API provides RESTful endpoints for managing software licenses, customers, products, and access control.

## Base URL

```
http://localhost:5000
https://localhost:5001
```

## Authentication

All API endpoints (except `/health`) require authentication using an API key passed in the `Auth_Key` header.

```bash
curl -H "Auth_Key: your-api-key-here" http://localhost:5000/api/customers
```

See the [Authentication Guide](authentication.md) for detailed information.

## Response Format

All responses follow a consistent format:

### Success Response

```json
{
  "data": { ... },
  "message": "Success message",
  "success": true
}
```

### Error Response

```json
{
  "message": "Error description",
  "errorCode": "ERROR_CODE",
  "success": false
}
```

## HTTP Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 OK | Request successful |
| 201 Created | Resource created successfully |
| 400 Bad Request | Invalid request data |
| 401 Unauthorized | Missing or invalid API key |
| 403 Forbidden | Insufficient permissions |
| 404 Not Found | Resource not found |
| 500 Internal Server Error | Server error |

## Pagination

List endpoints support pagination with the following query parameters:

| Parameter | Type | Default | Max | Description |
|-----------|------|---------|-----|-------------|
| page | integer | 1 | - | Page number (1-indexed) |
| pageSize | integer | 10 | 100 | Number of items per page |

### Pagination Response

```json
{
  "data": {
    "items": [...],
    "page": 1,
    "pageSize": 10,
    "totalCount": 50,
    "totalPages": 5
  }
}
```

## API Endpoints

### Health Check

- `GET /health` - Check API health status (no authentication required)

### Customers

- `POST /api/customers` - Create a new customer
- `GET /api/customers/{id}` - Get customer by ID
- `GET /api/customers` - List customers (paginated)
- `GET /api/customers/search?name={name}` - Search customers by name
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Products

- `POST /api/products` - Create a new product
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products` - List products (paginated)
- `GET /api/products/search?name={name}` - Search products by name
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### SKUs

- `POST /api/skus` - Create a new SKU
- `GET /api/skus/{id}` - Get SKU by ID
- `GET /api/skus` - List SKUs (paginated)
- `GET /api/skus/product/{productId}` - List SKUs by product
- `GET /api/skus/search?name={name}` - Search SKUs by name
- `PUT /api/skus/{id}` - Update SKU
- `DELETE /api/skus/{id}` - Delete SKU

### RSA Keys

- `POST /api/rsakeys` - Generate RSA key pair
- `GET /api/rsakeys/{id}` - Get RSA key by ID
- `GET /api/rsakeys/{id}/private` - Download private key
- `GET /api/rsakeys` - List RSA keys (paginated)
- `PUT /api/rsakeys/{id}` - Update RSA key
- `DELETE /api/rsakeys/{id}` - Delete RSA key

### Licenses

- `POST /api/licenses` - Create a new license
- `GET /api/licenses/{id}` - Get license by ID
- `GET /api/licenses` - List licenses (paginated)
- `GET /api/licenses/customer/{customerId}` - List licenses by customer
- `GET /api/licenses/product/{productId}` - List licenses by product
- `GET /api/licenses/status/{status}` - List licenses by status
- `PUT /api/licenses/{id}` - Update license
- `POST /api/licenses/{id}/revoke` - Revoke license
- `POST /api/licenses/validate` - Validate license key
- `POST /api/licenses/activate` - Activate license key
- `DELETE /api/licenses/{id}` - Delete license

### API Keys

- `POST /api/apikeys` - Create a new API key
- `GET /api/apikeys/{id}` - Get API key by ID
- `GET /api/apikeys` - List API keys (paginated)
- `PUT /api/apikeys/{id}` - Update API key
- `POST /api/apikeys/validate` - Validate API key
- `DELETE /api/apikeys/{id}` - Delete API key

## Common Request Examples

### Create Customer

```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -H "Auth_Key: your-api-key" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "organization": "Acme Corp"
  }'
```

### List Customers with Pagination

```bash
curl -H "Auth_Key: your-api-key" \
  "http://localhost:5000/api/customers?page=1&pageSize=20"
```

### Search Products

```bash
curl -H "Auth_Key: your-api-key" \
  "http://localhost:5000/api/products/search?name=Enterprise"
```

### Validate License Key

```bash
curl -X POST http://localhost:5000/api/licenses/validate \
  -H "Content-Type: application/json" \
  -H "Auth_Key: your-api-key" \
  -d '{
    "licenseKey": "LIC_1234567890abcdef"
  }'
```

## Rate Limiting

!!! info "Coming Soon"
    Rate limiting is not currently implemented but is planned for future releases.

## Versioning

The API currently does not use versioning. Breaking changes will be communicated in advance.

## Interactive Documentation

When running in development mode, Swagger UI is available at:

```
http://localhost:5000/swagger
```

Swagger provides:

- Interactive API exploration
- Request/response examples
- Schema documentation
- Try-it-out functionality

## Postman Collection

A comprehensive Postman collection is available for testing all endpoints. See the [Postman Collection Guide](postman.md) for details.

## Next Steps

- Review [Authentication](authentication.md) for API key management
- Explore [Endpoints](endpoints.md) for detailed endpoint documentation
- Import the [Postman Collection](postman.md) for interactive testing

# API Endpoints

Detailed documentation for all API endpoints.

!!! note "API Documentation"
    For the most up-to-date API documentation, refer to the Swagger UI at `http://localhost:5000/swagger` when running the application.

## Health Check

### Get Health Status

Check the health status of the API and database connectivity.

**Endpoint:** `GET /health`

**Authentication:** None required

**Response:**

```json
{
  "status": "Healthy",
  "version": "1.0.0",
  "duration": "00:00:00.0123456",
  "checks": {
    "postgresql": {
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0100000"
    },
    "database": {
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0050000"
    }
  }
}
```

## Customers

### Create Customer

Create a new customer account.

**Endpoint:** `POST /api/customers`

**Request Body:**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "organization": "Acme Corp"
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john@example.com",
  "organization": "Acme Corp",
  "isVisible": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

### Get Customer by ID

Retrieve a customer by their unique identifier.

**Endpoint:** `GET /api/customers/{id}`

**Response:** `200 OK`

### List Customers

List all customers with pagination.

**Endpoint:** `GET /api/customers?page=1&pageSize=10`

**Query Parameters:**

- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10, max: 100)

**Response:** `200 OK`

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5
}
```

### Search Customers

Search customers by name.

**Endpoint:** `GET /api/customers/search?name={name}`

**Response:** `200 OK`

### Update Customer

Update an existing customer.

**Endpoint:** `PUT /api/customers/{id}`

**Request Body:**

```json
{
  "name": "John Doe Updated",
  "organization": "New Corp"
}
```

**Response:** `200 OK`

### Delete Customer

Delete a customer by ID.

**Endpoint:** `DELETE /api/customers/{id}`

**Response:** `204 No Content`

## Products

### Create Product

Create a new product.

**Endpoint:** `POST /api/products`

**Request Body:**

```json
{
  "name": "Enterprise Suite",
  "productCode": "ENT-001",
  "description": "Enterprise software suite"
}
```

**Response:** `201 Created`

### Get Product by ID

**Endpoint:** `GET /api/products/{id}`

### List Products

**Endpoint:** `GET /api/products?page=1&pageSize=10`

### Search Products

**Endpoint:** `GET /api/products/search?name={name}`

### Update Product

**Endpoint:** `PUT /api/products/{id}`

### Delete Product

**Endpoint:** `DELETE /api/products/{id}`

## SKUs

### Create SKU

Create a new SKU for a product.

**Endpoint:** `POST /api/skus`

**Request Body:**

```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Professional Edition",
  "skuCode": "ENT-PRO-001",
  "description": "Professional tier with advanced features"
}
```

**Response:** `201 Created`

### Get SKU by ID

**Endpoint:** `GET /api/skus/{id}`

### List SKUs

**Endpoint:** `GET /api/skus?page=1&pageSize=10`

### List SKUs by Product

**Endpoint:** `GET /api/skus/product/{productId}`

### Search SKUs

**Endpoint:** `GET /api/skus/search?name={name}`

### Update SKU

**Endpoint:** `PUT /api/skus/{id}`

### Delete SKU

**Endpoint:** `DELETE /api/skus/{id}`

## RSA Keys

### Generate RSA Key Pair

Generate a new RSA key pair for license signing.

**Endpoint:** `POST /api/rsakeys`

**Request Body:**

```json
{
  "name": "Production Key 2024",
  "keySize": 2048,
  "createdBy": "admin@example.com"
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Production Key 2024",
  "publicKey": "-----BEGIN PUBLIC KEY-----\n...\n-----END PUBLIC KEY-----",
  "keySize": 2048,
  "createdBy": "admin@example.com",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### Get RSA Key by ID

**Endpoint:** `GET /api/rsakeys/{id}`

### Download Private Key

Download the encrypted private key.

**Endpoint:** `GET /api/rsakeys/{id}/private`

**Response:** File download

### List RSA Keys

**Endpoint:** `GET /api/rsakeys?page=1&pageSize=10`

### Update RSA Key

**Endpoint:** `PUT /api/rsakeys/{id}`

### Delete RSA Key

**Endpoint:** `DELETE /api/rsakeys/{id}`

## Licenses

### Create License

Create a new license for a customer.

**Endpoint:** `POST /api/licenses`

**Request Body:**

```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "skuId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "rsaKeyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "licenseType": "Standard",
  "expirationDate": "2025-12-31T23:59:59Z",
  "maxActivations": 5
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "skuId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "licenseKey": "LIC_1234567890abcdef",
  "signedPayload": "...",
  "licenseType": "Standard",
  "expirationDate": "2025-12-31T23:59:59Z",
  "maxActivations": 5,
  "currentActivations": 0,
  "status": 0,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### Get License by ID

**Endpoint:** `GET /api/licenses/{id}`

### List Licenses

**Endpoint:** `GET /api/licenses?page=1&pageSize=10`

### List Licenses by Customer

**Endpoint:** `GET /api/licenses/customer/{customerId}`

### List Licenses by Product

**Endpoint:** `GET /api/licenses/product/{productId}`

### List Licenses by Status

**Endpoint:** `GET /api/licenses/status/{status}`

**Status Values:**

- `0` - Active
- `1` - Expired
- `2` - Revoked

### Update License

**Endpoint:** `PUT /api/licenses/{id}`

### Revoke License

Revoke an active license.

**Endpoint:** `POST /api/licenses/{id}/revoke`

**Response:** `200 OK`

### Validate License Key

Validate a license key without activating it.

**Endpoint:** `POST /api/licenses/validate`

**Request Body:**

```json
{
  "licenseKey": "LIC_1234567890abcdef"
}
```

**Response:** `200 OK`

```json
{
  "isValid": true,
  "licenseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Active",
  "expirationDate": "2025-12-31T23:59:59Z",
  "remainingActivations": 5
}
```

### Activate License Key

Activate a license key and increment activation count.

**Endpoint:** `POST /api/licenses/activate`

**Request Body:**

```json
{
  "licenseKey": "LIC_1234567890abcdef",
  "machineId": "MACHINE-12345"
}
```

**Response:** `200 OK`

```json
{
  "success": true,
  "activationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "currentActivations": 1,
  "maxActivations": 5
}
```

### Delete License

**Endpoint:** `DELETE /api/licenses/{id}`

## API Keys

### Create API Key

Create a new API key for authentication.

**Endpoint:** `POST /api/apikeys`

**Request Body:**

```json
{
  "name": "Production API Key",
  "role": "Admin",
  "createdBy": "admin@example.com"
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "key": "LMA_1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t",
  "name": "Production API Key",
  "role": "Admin",
  "isActive": true,
  "createdBy": "admin@example.com",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

!!! warning "Important"
    The plain-text API key is only returned during creation. Store it securely.

### Get API Key by ID

**Endpoint:** `GET /api/apikeys/{id}`

### List API Keys

**Endpoint:** `GET /api/apikeys?page=1&pageSize=10`

### Update API Key

**Endpoint:** `PUT /api/apikeys/{id}`

**Request Body:**

```json
{
  "name": "Updated Key Name",
  "role": "User",
  "isActive": false
}
```

### Validate API Key

Validate an API key.

**Endpoint:** `POST /api/apikeys/validate`

**Request Body:**

```json
{
  "apiKey": "LMA_1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t"
}
```

**Response:** `200 OK`

```json
{
  "isValid": true,
  "apiKeyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "Admin",
  "name": "Production API Key"
}
```

### Delete API Key

**Endpoint:** `DELETE /api/apikeys/{id}`

**Response:** `204 No Content`

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request

```json
{
  "message": "Validation error description",
  "errorCode": "VALIDATION_ERROR"
}
```

### 401 Unauthorized

```json
{
  "message": "Missing or invalid Auth_Key header",
  "errorCode": "UNAUTHORIZED"
}
```

### 403 Forbidden

```json
{
  "message": "Insufficient permissions",
  "errorCode": "FORBIDDEN"
}
```

### 404 Not Found

```json
{
  "message": "Resource not found",
  "errorCode": "NOT_FOUND"
}
```

### 500 Internal Server Error

```json
{
  "message": "An internal error occurred",
  "errorCode": "INTERNAL_ERROR"
}
```

## Next Steps

- Review [Authentication](authentication.md) for API key management
- Import the [Postman Collection](postman.md) for interactive testing
- Explore the [Database Schema](../architecture/database.md) to understand data relationships

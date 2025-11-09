# Quick Start Guide

Get the License Management System (backend API and frontend UI) up and running in minutes.

## Prerequisites

Before you begin, ensure you have:

### Backend
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- [PostgreSQL 12+](https://www.postgresql.org/download/) installed and running
- A PostgreSQL database instance accessible

### Frontend
- [Node.js 18+](https://nodejs.org/) installed
- npm or yarn package manager

## Installation Steps

### 1. Clone the Repository

```bash
git clone <repository-url>
cd LicenseManagementApi
```

### 2. Configure Database Connection

Update the connection string in `LicenseManagementApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LicenseManagement;Username=postgres;Password=your_password;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100"
  }
}
```

!!! tip "Environment Variables"
    For production, use environment variables:
    ```bash
    export ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=LicenseManagement;Username=your_user;Password=your_password"
    ```

### 3. Configure Cryptography Settings

!!! warning "Security"
    Change the private key passphrase in production:

```json
{
  "Cryptography": {
    "PrivateKeyPassphrase": "CHANGE_THIS_IN_PRODUCTION_USE_SECURE_KEY_VAULT"
  }
}
```

For production, use Azure Key Vault, AWS Secrets Manager, or similar secure storage.

### 4. Create the Database

The application will automatically create the database and apply migrations on startup. Ensure your PostgreSQL instance is running and the connection string is correct.

Alternatively, you can manually create the database:

```bash
createdb -U postgres LicenseManagement
```

### 5. Run the Application

```bash
cd LicenseManagementApi
dotnet restore
dotnet run
```

The API will be available at:

- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

### 6. Verify Health Status

Check the health endpoint:

```bash
curl http://localhost:5000/health
```

Expected response:

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

## Development Setup

### Running with Hot Reload

```bash
dotnet watch run --project LicenseManagementApi
```

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName --project LicenseManagementApi
```

Apply migrations manually:
```bash
dotnet ef database update --project LicenseManagementApi
```

Remove last migration:
```bash
dotnet ef migrations remove --project LicenseManagementApi
```

### Running Tests

```bash
dotnet test
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## Creating Your First API Key

All API endpoints (except `/health`) require authentication. To create your first API key:

1. Temporarily disable authentication middleware in `Program.cs` (comment out the authentication middleware line)
2. Start the application
3. Create an API key using the POST `/api/apikeys` endpoint:

```bash
curl -X POST http://localhost:5000/api/apikeys \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Bootstrap Admin Key",
    "role": "Admin",
    "createdBy": "system"
  }'
```

4. Save the returned API key (it's only shown once)
5. Re-enable authentication middleware
6. Restart the application

### Using API Keys

Include the API key in all requests:

```bash
curl -H "Auth_Key: your-api-key-here" http://localhost:5000/api/customers
```

## Frontend Setup

### 1. Navigate to Frontend Directory

```bash
cd vue-ui-frontend
```

### 2. Install Dependencies

```bash
npm install
```

### 3. Configure API URL

The frontend is configured to connect to `http://localhost:5000` by default. If your backend runs on a different port, update the API base URL in `src/services/api.ts`.

### 4. Run the Development Server

```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

### 5. Access the Application

Open your browser and navigate to `http://localhost:5173`. You should see the License Management System home page.

## Running Both Backend and Frontend

### Terminal 1 - Backend
```bash
cd LicenseManagementApi
dotnet run
```

### Terminal 2 - Frontend
```bash
cd vue-ui-frontend
npm run dev
```

## Building for Production

### Backend
```bash
cd LicenseManagementApi
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd vue-ui-frontend
npm run build
```

The built files will be in `vue-ui-frontend/dist/`

## Next Steps

- Review the [Configuration Guide](configuration.md) for production settings
- Explore the [API Reference](../api/overview.md) to understand available endpoints
- Learn about [Frontend Components](../frontend/components.md)
- Import the [Postman Collection](../api/postman.md) for interactive testing
- Learn about [Authentication](../api/authentication.md) and authorization

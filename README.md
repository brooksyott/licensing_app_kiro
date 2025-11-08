# License Management API

A secure, scalable backend system built on ASP.NET Core 8.0 with PostgreSQL for managing software licenses, license keys, customers, and access control.

## Overview

The License Management API enables software vendors to:
- Manage customers and their license entitlements
- Define products and SKUs for different software offerings
- Generate and manage RSA key pairs for license signing
- Issue digitally signed licenses with activation controls
- Validate and activate license keys
- Control API access with role-based authentication

## Features

- **Customer Management**: Create and manage customer accounts with organization details
- **Product & SKU Management**: Organize software products and their variants
- **RSA Key Management**: Generate and securely store RSA key pairs for license signing
- **License Generation**: Issue digitally signed licenses with expiration and activation limits
- **License Validation**: Validate license keys with cryptographic security
- **API Key Authentication**: Role-based access control with cached authentication
- **Health Monitoring**: Built-in health checks for system status
- **Structured Logging**: Comprehensive logging with Serilog
- **Database Migrations**: Automatic schema evolution with Entity Framework Core

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Npgsql provider
- **ORM**: Entity Framework Core 9.0
- **Logging**: Serilog with console and file sinks
- **Authentication**: Custom API key-based authentication with role-based authorization
- **Cryptography**: RSA digital signatures for license signing
- **Health Checks**: ASP.NET Core Health Checks with PostgreSQL connectivity verification

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- A PostgreSQL database instance running and accessible

## Getting Started

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

For production, use environment variables or a secure configuration provider:

```bash
export ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=LicenseManagement;Username=your_user;Password=your_password"
```

### 3. Configure Cryptography Settings

**IMPORTANT**: Change the private key passphrase in production:

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

## Authentication

All API endpoints (except `/health`) require authentication using an API key passed in the `Auth_Key` header.

### Creating Your First API Key

1. Temporarily disable authentication middleware in `Program.cs` (comment out the authentication middleware line)
2. Start the application
3. Create an API key using the POST `/api/apikeys` endpoint
4. Save the returned API key (it's only shown once)
5. Re-enable authentication middleware
6. Restart the application

### Using API Keys

Include the API key in all requests:

```bash
curl -H "Auth_Key: your-api-key-here" http://localhost:5000/api/customers
```

### Roles

The API supports role-based authorization:
- **Admin**: Full access to all endpoints
- **User**: Limited access (customize based on your needs)

## API Documentation

### Using Swagger UI

Navigate to `http://localhost:5000/swagger` when running in development mode to explore the API interactively.

### Using Postman

Import the provided Postman collection:
1. Open Postman
2. Click "Import"
3. Select `LicenseManagementApi.postman_collection.json`
4. Configure the `baseUrl` and `authKey` environment variables
5. See `POSTMAN_COLLECTION_GUIDE.md` for detailed usage instructions

### API Endpoints

See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for complete endpoint documentation with request/response examples.

## Configuration

### Connection Pooling

The default configuration uses connection pooling for optimal performance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LicenseManagement;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;Connection Idle Lifetime=300;Connection Pruning Interval=10"
  }
}
```

### Logging

Configure logging levels in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    }
  }
}
```

Logs are written to:
- Console output
- `logs/log-YYYYMMDD.txt` (rolling daily)

### Caching

Authentication results are cached in memory for 5 minutes to optimize performance. For horizontal scaling across multiple instances, configure a distributed cache (Redis or SQL Server) in `Program.cs`.

## Database Schema

See [DATABASE_SCHEMA.md](DATABASE_SCHEMA.md) for detailed database schema documentation.

## Security Considerations

1. **Change Default Credentials**: Update the database password and cryptography passphrase
2. **Use HTTPS**: Always use HTTPS in production
3. **Secure API Keys**: Store API keys securely and rotate them regularly
4. **Environment Variables**: Use environment variables or secure vaults for sensitive configuration
5. **Connection Strings**: Never commit connection strings with real credentials to source control
6. **Private Keys**: RSA private keys are encrypted at rest using the configured passphrase
7. **License Keys**: License keys are hashed with salt before storage

## Deployment

### Docker Deployment

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LicenseManagementApi/LicenseManagementApi.csproj", "LicenseManagementApi/"]
RUN dotnet restore "LicenseManagementApi/LicenseManagementApi.csproj"
COPY . .
WORKDIR "/src/LicenseManagementApi"
RUN dotnet build "LicenseManagementApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LicenseManagementApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LicenseManagementApi.dll"]
```

Build and run:
```bash
docker build -t license-management-api .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="your-connection-string" license-management-api
```

### Production Checklist

- [ ] Update database connection string
- [ ] Change cryptography passphrase and store in secure vault
- [ ] Configure HTTPS with valid certificates
- [ ] Set up distributed caching for horizontal scaling
- [ ] Configure production logging (consider centralized logging)
- [ ] Set up database backups
- [ ] Configure monitoring and alerting
- [ ] Review and configure CORS policies if needed
- [ ] Set up rate limiting
- [ ] Configure reverse proxy (nginx, IIS, etc.)

## Troubleshooting

### Database Connection Issues

**Error**: "Could not connect to the database"

**Solution**: 
- Verify PostgreSQL is running: `pg_isready -h localhost -p 5432`
- Check connection string credentials
- Ensure database exists or allow auto-creation
- Check firewall rules

### Migration Issues

**Error**: "Pending migrations"

**Solution**: The application applies migrations automatically on startup. If this fails, apply manually:
```bash
dotnet ef database update --project LicenseManagementApi
```

### Authentication Issues

**Error**: "401 Unauthorized"

**Solution**:
- Verify the `Auth_Key` header is included in the request
- Check that the API key exists and is active
- Ensure the API key hasn't been deleted (cache may need to expire)

## Performance

- **Health Check Response Time**: Target < 200ms
- **API Response Time**: Typical < 100ms for CRUD operations
- **Connection Pooling**: Configured for 5-100 concurrent connections
- **Authentication Caching**: 5-minute cache reduces database load
- **Database Indexes**: Optimized for common query patterns

## Documentation

Comprehensive documentation is available using MkDocs:

### View Documentation Locally

```bash
# Install dependencies (first time only)
pip install -r requirements.txt

# Serve documentation with live reload
mkdocs serve
```

Or use the PowerShell scripts:

```powershell
# Serve documentation (with auto-install)
.\serve-docs.ps1

# Build static site
.\build-docs.ps1
```

Documentation will be available at `http://127.0.0.1:8000`

### Documentation Contents

- **Getting Started** - Quick start, configuration, and deployment guides
- **API Reference** - Complete API documentation with examples
- **Architecture** - Database schema and design documentation
- **Development** - Requirements, testing, and contributing guides

### Build Static Documentation

```bash
mkdocs build
```

The built site will be in the `site/` directory.

## License

[Add your license information here]

## Support

[Add support contact information here]

## Contributing

See the [Contributing Guide](docs/development/contributing.md) for detailed information on how to contribute to this project.

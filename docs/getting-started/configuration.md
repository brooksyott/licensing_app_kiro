# Configuration Guide

This guide covers configuration options for the License Management API.

## Connection Strings

### Development

For local development, configure the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LicenseManagement;Username=postgres;Password=postgres"
  }
}
```

### Production

For production, use environment variables or a secure configuration provider:

```bash
export ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=LicenseManagement;Username=your_user;Password=your_password"
```

### Connection Pooling

The default configuration uses connection pooling for optimal performance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LicenseManagement;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;Connection Idle Lifetime=300;Connection Pruning Interval=10"
  }
}
```

**Connection Pool Parameters:**

- `Pooling=true` - Enable connection pooling
- `Minimum Pool Size=5` - Minimum number of connections in the pool
- `Maximum Pool Size=100` - Maximum number of connections in the pool
- `Connection Idle Lifetime=300` - Seconds before idle connections are closed
- `Connection Pruning Interval=10` - Seconds between pool cleanup operations

## Cryptography Settings

### Private Key Passphrase

!!! danger "Security Critical"
    Always change the default passphrase in production!

```json
{
  "Cryptography": {
    "PrivateKeyPassphrase": "CHANGE_THIS_IN_PRODUCTION_USE_SECURE_KEY_VAULT"
  }
}
```

### Secure Storage Options

For production environments, use a secure key management service:

=== "Azure Key Vault"
    ```csharp
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
    ```

=== "AWS Secrets Manager"
    ```csharp
    builder.Configuration.AddSecretsManager(
        configurator: options =>
        {
            options.SecretFilter = entry => entry.Name.StartsWith("LicenseManagement");
        });
    ```

=== "Environment Variables"
    ```bash
    export Cryptography__PrivateKeyPassphrase="your-secure-passphrase"
    ```

## Logging Configuration

### Log Levels

Configure logging levels in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

**Log Levels:**

- `Verbose` - Most detailed logging
- `Debug` - Debugging information
- `Information` - General informational messages
- `Warning` - Warning messages
- `Error` - Error messages
- `Fatal` - Critical failures

### Log Output

Logs are written to:

- **Console output** - For development and container environments
- **File output** - `logs/log-YYYYMMDD.txt` (rolling daily)

### Production Logging

For production, consider centralized logging:

=== "Seq"
    ```json
    {
      "Serilog": {
        "WriteTo": [
          {
            "Name": "Seq",
            "Args": {
              "serverUrl": "http://seq-server:5341",
              "apiKey": "your-api-key"
            }
          }
        ]
      }
    }
    ```

=== "Application Insights"
    ```json
    {
      "Serilog": {
        "WriteTo": [
          {
            "Name": "ApplicationInsights",
            "Args": {
              "instrumentationKey": "your-instrumentation-key"
            }
          }
        ]
      }
    }
    ```

## Caching Configuration

### In-Memory Cache (Default)

The default configuration uses in-memory caching for authentication:

```csharp
builder.Services.AddMemoryCache();
```

**Cache Settings:**

- **Duration**: 5 minutes
- **Scope**: Single application instance
- **Use Case**: Development and single-instance deployments

### Distributed Cache (Production)

For horizontal scaling across multiple instances, configure a distributed cache:

=== "Redis"
    ```csharp
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "LicenseManagement_";
    });
    ```

=== "SQL Server"
    ```csharp
    builder.Services.AddDistributedSqlServerCache(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("CacheConnection");
        options.SchemaName = "dbo";
        options.TableName = "Cache";
    });
    ```

## CORS Configuration

Configure CORS policies for web applications:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins("https://your-web-app.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowWebApp");
```

## Health Check Configuration

Health checks are configured by default:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql")
    .AddDbContextCheck<LicenseManagementDbContext>(name: "database");
```

**Health Check Endpoints:**

- `/health` - Returns health status with database connectivity checks

## Environment-Specific Configuration

### Development

`appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LicenseManagement;Username=postgres;Password=postgres"
  }
}
```

### Production

`appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Use environment variables for sensitive data:

```bash
export ConnectionStrings__DefaultConnection="production-connection-string"
export Cryptography__PrivateKeyPassphrase="production-passphrase"
```

## Security Configuration

### HTTPS

Always use HTTPS in production:

```csharp
app.UseHttpsRedirection();
```

Configure HTTPS certificates:

=== "Development"
    ```bash
    dotnet dev-certs https --trust
    ```

=== "Production"
    Use valid SSL/TLS certificates from a trusted CA

### API Key Security

- Store API keys securely (never in source control)
- Rotate API keys regularly
- Use role-based access control
- Monitor API key usage via `LastUsedAt` field

## Performance Tuning

### Database Connection Pool

Adjust pool size based on expected load:

```
Minimum Pool Size=10;Maximum Pool Size=200
```

### Authentication Cache

Adjust cache duration in `AuthenticationService.cs`:

```csharp
var cacheOptions = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Adjust as needed
};
```

## Configuration Validation

Validate configuration on startup:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured");
}

var passphrase = builder.Configuration["Cryptography:PrivateKeyPassphrase"];
if (passphrase == "CHANGE_THIS_IN_PRODUCTION_USE_SECURE_KEY_VAULT")
{
    throw new InvalidOperationException("Default cryptography passphrase must be changed in production");
}
```

## Next Steps

- Review [Deployment Guide](deployment.md) for production deployment
- Learn about [Authentication](../api/authentication.md) configuration
- Explore [Database Schema](../architecture/database.md) for data model details

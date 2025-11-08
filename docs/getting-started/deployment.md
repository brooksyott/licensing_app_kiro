# Deployment Guide

This guide covers deploying the License Management API to production environments.

## Pre-Deployment Checklist

Before deploying to production, ensure you have:

- [ ] Updated database connection string
- [ ] Changed cryptography passphrase and stored in secure vault
- [ ] Configured HTTPS with valid certificates
- [ ] Set up distributed caching for horizontal scaling
- [ ] Configured production logging (consider centralized logging)
- [ ] Set up database backups
- [ ] Configured monitoring and alerting
- [ ] Reviewed and configured CORS policies if needed
- [ ] Set up rate limiting
- [ ] Configured reverse proxy (nginx, IIS, etc.)

## Docker Deployment

### Create Dockerfile

Create a `Dockerfile` in the project root:

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

### Build and Run

Build the Docker image:

```bash
docker build -t license-management-api:latest .
```

Run the container:

```bash
docker run -d \
  -p 5000:80 \
  -e ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=LicenseManagement;Username=postgres;Password=your_password" \
  -e Cryptography__PrivateKeyPassphrase="your-secure-passphrase" \
  --name license-api \
  license-management-api:latest
```

### Docker Compose

Create a `docker-compose.yml` file:

```yaml
version: '3.8'

services:
  db:
    image: postgres:15
    environment:
      POSTGRES_DB: LicenseManagement
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: your_password
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build: .
    ports:
      - "5000:80"
    environment:
      ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=LicenseManagement;Username=postgres;Password=your_password"
      Cryptography__PrivateKeyPassphrase: "your-secure-passphrase"
      ASPNETCORE_ENVIRONMENT: Production
    depends_on:
      db:
        condition: service_healthy
    restart: unless-stopped

volumes:
  postgres_data:
```

Run with Docker Compose:

```bash
docker-compose up -d
```

## Cloud Deployment

### Azure App Service

#### Prerequisites

- Azure CLI installed
- Azure subscription

#### Deployment Steps

1. **Create Resource Group**

```bash
az group create --name license-api-rg --location eastus
```

2. **Create App Service Plan**

```bash
az appservice plan create \
  --name license-api-plan \
  --resource-group license-api-rg \
  --sku B1 \
  --is-linux
```

3. **Create Web App**

```bash
az webapp create \
  --name license-management-api \
  --resource-group license-api-rg \
  --plan license-api-plan \
  --runtime "DOTNETCORE:8.0"
```

4. **Configure Connection String**

```bash
az webapp config connection-string set \
  --name license-management-api \
  --resource-group license-api-rg \
  --connection-string-type PostgreSQL \
  --settings DefaultConnection="Host=your-db.postgres.database.azure.com;Database=LicenseManagement;Username=admin;Password=your_password"
```

5. **Configure App Settings**

```bash
az webapp config appsettings set \
  --name license-management-api \
  --resource-group license-api-rg \
  --settings Cryptography__PrivateKeyPassphrase="your-secure-passphrase"
```

6. **Deploy Application**

```bash
az webapp deployment source config-zip \
  --name license-management-api \
  --resource-group license-api-rg \
  --src ./publish.zip
```

### AWS Elastic Beanstalk

#### Prerequisites

- AWS CLI installed
- EB CLI installed
- AWS account

#### Deployment Steps

1. **Initialize EB Application**

```bash
eb init -p "64bit Amazon Linux 2 v2.5.0 running .NET Core" license-management-api
```

2. **Create Environment**

```bash
eb create license-api-prod \
  --database.engine postgres \
  --database.username admin \
  --database.password your_password
```

3. **Set Environment Variables**

```bash
eb setenv \
  ConnectionStrings__DefaultConnection="Host=your-db.region.rds.amazonaws.com;Database=LicenseManagement;Username=admin;Password=your_password" \
  Cryptography__PrivateKeyPassphrase="your-secure-passphrase"
```

4. **Deploy Application**

```bash
eb deploy
```

### Google Cloud Run

#### Prerequisites

- Google Cloud SDK installed
- Google Cloud project

#### Deployment Steps

1. **Build Container**

```bash
gcloud builds submit --tag gcr.io/your-project-id/license-management-api
```

2. **Deploy to Cloud Run**

```bash
gcloud run deploy license-management-api \
  --image gcr.io/your-project-id/license-management-api \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --set-env-vars ConnectionStrings__DefaultConnection="Host=your-db;Database=LicenseManagement;Username=admin;Password=your_password" \
  --set-env-vars Cryptography__PrivateKeyPassphrase="your-secure-passphrase"
```

## Reverse Proxy Configuration

### Nginx

Create `/etc/nginx/sites-available/license-api`:

```nginx
server {
    listen 80;
    server_name api.yourdomain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable the site:

```bash
sudo ln -s /etc/nginx/sites-available/license-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### IIS

1. Install ASP.NET Core Hosting Bundle
2. Create a new site in IIS Manager
3. Configure Application Pool to "No Managed Code"
4. Set physical path to published application
5. Configure bindings (HTTP/HTTPS)

## Database Setup

### PostgreSQL on Cloud

=== "Azure Database for PostgreSQL"
    ```bash
    az postgres server create \
      --name license-db \
      --resource-group license-api-rg \
      --location eastus \
      --admin-user admin \
      --admin-password your_password \
      --sku-name B_Gen5_1
    ```

=== "AWS RDS PostgreSQL"
    ```bash
    aws rds create-db-instance \
      --db-instance-identifier license-db \
      --db-instance-class db.t3.micro \
      --engine postgres \
      --master-username admin \
      --master-user-password your_password \
      --allocated-storage 20
    ```

=== "Google Cloud SQL"
    ```bash
    gcloud sql instances create license-db \
      --database-version=POSTGRES_14 \
      --tier=db-f1-micro \
      --region=us-central1
    ```

### Database Migrations

Apply migrations on deployment:

```bash
dotnet ef database update --project LicenseManagementApi
```

Or enable automatic migrations in `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LicenseManagementDbContext>();
    db.Database.Migrate();
}
```

## Monitoring and Logging

### Application Insights (Azure)

Install package:

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

Configure in `Program.cs`:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### CloudWatch (AWS)

Install package:

```bash
dotnet add package AWS.Logger.SerilogSink
```

Configure in `appsettings.json`:

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "AWSSerilog",
        "Args": {
          "logGroup": "/aws/elasticbeanstalk/license-api",
          "region": "us-east-1"
        }
      }
    ]
  }
}
```

## Security Hardening

### HTTPS Configuration

Enforce HTTPS:

```csharp
app.UseHttpsRedirection();
app.UseHsts();
```

### Rate Limiting

Install package:

```bash
dotnet add package AspNetCoreRateLimit
```

Configure in `Program.cs`:

```csharp
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

app.UseIpRateLimiting();
```

### Security Headers

Add security headers middleware:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});
```

## Backup and Recovery

### Database Backups

=== "PostgreSQL"
    ```bash
    # Backup
    pg_dump -h localhost -U postgres LicenseManagement > backup.sql
    
    # Restore
    psql -h localhost -U postgres LicenseManagement < backup.sql
    ```

=== "Azure"
    ```bash
    az postgres server-backup create \
      --resource-group license-api-rg \
      --server-name license-db \
      --backup-name manual-backup
    ```

=== "AWS"
    ```bash
    aws rds create-db-snapshot \
      --db-instance-identifier license-db \
      --db-snapshot-identifier license-db-snapshot
    ```

### Automated Backups

Configure automated backups in your cloud provider:

- **Azure**: Automatic backups with 7-35 day retention
- **AWS**: Automated backups with 1-35 day retention
- **GCP**: Automated backups with configurable retention

## Performance Optimization

### Connection Pooling

Optimize connection pool settings:

```
Minimum Pool Size=10;Maximum Pool Size=200;Connection Idle Lifetime=300
```

### Caching

Use distributed cache for multi-instance deployments:

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis-server:6379";
});
```

### CDN for Static Assets

If serving static content, use a CDN:

- Azure CDN
- CloudFront (AWS)
- Cloud CDN (GCP)

## Troubleshooting

### Health Check Failures

Check database connectivity:

```bash
curl https://your-api.com/health
```

### Connection Issues

Verify connection string and firewall rules:

```bash
psql -h your-db-host -U admin -d LicenseManagement
```

### Performance Issues

Monitor application metrics:

- Response times
- Database query performance
- Connection pool utilization
- Memory usage

## Next Steps

- Set up [monitoring and alerting](../development/testing.md)
- Review [security best practices](configuration.md#security-configuration)
- Configure [automated backups](#backup-and-recovery)
- Implement [CI/CD pipeline](../development/contributing.md)

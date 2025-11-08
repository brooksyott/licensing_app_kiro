using Serilog;
using LicenseManagementApi.Middleware;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Reflection;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Auth_Key", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Auth_Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "API Key authentication. Enter your API key in the format: LMA_your_key_here"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Auth_Key"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add memory cache for authentication
// Note: For horizontal scaling across multiple instances, consider using a distributed cache
// such as Redis (AddStackExchangeRedisCache) or SQL Server (AddDistributedSqlServerCache)
// to share authentication cache across all instances
builder.Services.AddMemoryCache();

// Configure health checks with PostgreSQL connectivity verification
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "postgresql" },
        timeout: TimeSpan.FromMilliseconds(200))
    .AddDbContextCheck<LicenseManagementDbContext>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db" });

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<LicenseManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<LicenseManagementApi.Services.ICustomerService, LicenseManagementApi.Services.CustomerService>();
builder.Services.AddScoped<LicenseManagementApi.Services.IProductService, LicenseManagementApi.Services.ProductService>();
builder.Services.AddScoped<LicenseManagementApi.Services.ISkuService, LicenseManagementApi.Services.SkuService>();
builder.Services.AddScoped<LicenseManagementApi.Services.ICryptographyService, LicenseManagementApi.Services.CryptographyService>();
builder.Services.AddScoped<LicenseManagementApi.Services.IRsaKeyService, LicenseManagementApi.Services.RsaKeyService>();
builder.Services.AddScoped<LicenseManagementApi.Services.ILicenseKeyGenerator, LicenseManagementApi.Services.LicenseKeyGenerator>();
builder.Services.AddScoped<LicenseManagementApi.Services.ILicenseService, LicenseManagementApi.Services.LicenseService>();
builder.Services.AddScoped<LicenseManagementApi.Services.IApiKeyService, LicenseManagementApi.Services.ApiKeyService>();
builder.Services.AddScoped<LicenseManagementApi.Services.IAuthenticationService, LicenseManagementApi.Services.AuthenticationService>();

var app = builder.Build();

// Apply pending migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagementDbContext>();
    try
    {
        Log.Information("Applying database migrations...");
        dbContext.Database.Migrate();
        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while applying database migrations");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Authentication middleware (health check endpoints are exempted within the middleware)
app.UseMiddleware<LicenseManagementApi.Middleware.AuthenticationMiddleware>();

app.UseSerilogRequestLogging();

// Map health check endpoint with custom response writer
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var stopwatch = Stopwatch.StartNew();
        
        context.Response.ContentType = "application/json";
        
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "1.0.0";
        
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Version = version,
            Duration = report.TotalDuration,
            Checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckDetail
                {
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    Duration = entry.Value.Duration
                })
        };
        
        stopwatch.Stop();
        
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions));
    }
});

app.MapControllers();

try
{
    Log.Information("Starting License Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

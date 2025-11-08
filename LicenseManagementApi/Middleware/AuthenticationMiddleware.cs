using System.Net;
using System.Text.Json;
using LicenseManagementApi.Services;

namespace LicenseManagementApi.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
    {
        // Skip authentication for health check endpoints
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Get Auth_Key from header
        if (!context.Request.Headers.TryGetValue("Auth_Key", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Request to {Path} missing Auth_Key header", context.Request.Path);
            await WriteUnauthorizedResponse(context, "Missing or invalid Auth_Key header");
            return;
        }

        // Authenticate the API key
        var authResult = await authenticationService.AuthenticateAsync(apiKey!);

        if (!authResult.IsSuccess)
        {
            _logger.LogError("Authentication service error: {ErrorMessage}", authResult.ErrorMessage);
            await WriteUnauthorizedResponse(context, "Authentication failed");
            return;
        }

        if (!authResult.Data.IsAuthenticated)
        {
            _logger.LogWarning("Invalid API key attempted for {Path}", context.Request.Path);
            await WriteUnauthorizedResponse(context, "Invalid API key");
            return;
        }

        // Store authentication information in HttpContext for use in controllers
        context.Items["ApiKeyId"] = authResult.Data.ApiKeyId;
        context.Items["Role"] = authResult.Data.Role;
        context.Items["Name"] = authResult.Data.Name;

        await _next(context);
    }

    private static Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var response = new
        {
            message,
            errorCode = "UNAUTHORIZED"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}

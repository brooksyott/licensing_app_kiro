using LicenseManagementApi.Models;

namespace LicenseManagementApi.Services;

public interface IAuthenticationService
{
    Task<ServiceResult<AuthenticationResult>> AuthenticateAsync(string apiKey);
    Task InvalidateCacheAsync(string apiKey);
}

public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public Guid? ApiKeyId { get; set; }
    public string? Role { get; set; }
    public string? Name { get; set; }
}

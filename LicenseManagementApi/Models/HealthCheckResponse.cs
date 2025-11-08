namespace LicenseManagementApi.Models;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Dictionary<string, HealthCheckDetail> Checks { get; set; } = new();
    public TimeSpan Duration { get; set; }
}

public class HealthCheckDetail
{
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
}

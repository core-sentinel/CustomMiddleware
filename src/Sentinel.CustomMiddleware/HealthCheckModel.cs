namespace Sentinel.CustomMiddleware;

public class HealthCheckModel
{
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
}

using BetterTests.Infrastructure.Persistence;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BetterTests.Infrastructure.HealthChecks;

public class DatabaseHealthCheck(AppDbContext context, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<DatabaseHealthCheck> _logger = logger;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database connection successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}

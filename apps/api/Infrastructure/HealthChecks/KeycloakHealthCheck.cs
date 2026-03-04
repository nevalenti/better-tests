using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BetterTests.Infrastructure.HealthChecks;

public class KeycloakHealthCheck(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakHealthCheck> logger) : IHealthCheck
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<KeycloakHealthCheck> _logger = logger;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Url"];
            var realm = _configuration["Keycloak:Realm"];

            if (string.IsNullOrEmpty(keycloakUrl) || string.IsNullOrEmpty(realm))
            {
                return HealthCheckResult.Degraded("Keycloak configuration missing");
            }

            var wellKnownUrl = $"{keycloakUrl.TrimEnd('/')}/realms/{realm}/.well-known/openid-configuration";

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var response = await _httpClient.GetAsync(wellKnownUrl, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Keycloak connection successful");
            }

            _logger.LogWarning("Keycloak health check failed with status {StatusCode}", response.StatusCode);
            return HealthCheckResult.Degraded($"Keycloak returned status {response.StatusCode}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Keycloak health check timed out");
            return HealthCheckResult.Degraded("Keycloak connection timeout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keycloak health check failed");
            return HealthCheckResult.Unhealthy("Keycloak connection failed", ex);
        }
    }
}

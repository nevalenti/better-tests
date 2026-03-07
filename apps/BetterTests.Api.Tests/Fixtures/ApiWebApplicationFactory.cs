using System.Security.Claims;
using System.Text.Encodings.Web;

using BetterTests.Infrastructure.HealthChecks;
using BetterTests.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BetterTests.Api.Tests.Fixtures;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "UseInMemoryDatabase", "true" }
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                            d.ServiceType.FullName?.StartsWith("Npgsql") == true ||
                            d.ServiceType == typeof(AppDbContext) ||
                            (d.ServiceType.IsGenericType &&
                            (d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                            d.ServiceType.GetGenericTypeDefinition().FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName).EnableSensitiveDataLogging());

            var authzDescriptors = services
                .Where(d => d.ServiceType.Name.Contains("Authorization"))
                .ToList();
            foreach (var descriptor in authzDescriptors)
                services.Remove(descriptor);

            services.AddHttpClient<KeycloakHealthCheck>()
                .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());

            services.AddAuthorizationBuilder()
                .AddPolicy("AuthenticatedUsers", policy => policy.RequireAssertion(_ => true));

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);

            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });
        });
    }
}

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim("preferred_username", "test-user"),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("{\"issuer\":\"http://localhost:8080/realms/better-tests\"}")
        });
    }
}

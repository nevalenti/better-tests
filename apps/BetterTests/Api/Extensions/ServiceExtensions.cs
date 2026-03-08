using BetterTests.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BetterTests.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

        return services.AddDbContext<AppDbContext>(options =>
        {
            if (useInMemory)
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            else
            {
                var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
                    ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is required.");

                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

                options.UseNpgsql(connectionString + password);
            }
        });
    }

    public static IServiceCollection AddKeycloakAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var keycloak = configuration.GetSection("Keycloak");
        var url = keycloak["Url"] ?? "http://localhost:8080";
        var realm = keycloak["Realm"] ?? "better-tests";
        var clientId = keycloak["ClientId"] ?? "better-tests-api";
        var authority = $"{url}/realms/{realm}";

        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.Audience = clientId;
            options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
            options.RequireHttpsMetadata = !environment.IsDevelopment() && authority.StartsWith("https://");
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(10),
                NameClaimType = "preferred_username",
                RoleClaimType = "realm_access"
            };
        })
        .Services;
    }
}

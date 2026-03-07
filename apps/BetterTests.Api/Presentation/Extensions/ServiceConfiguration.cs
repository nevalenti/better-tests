using Asp.Versioning;

using BetterTests.Application.Mappings;
using BetterTests.Infrastructure.HealthChecks;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.RateLimiting;

namespace BetterTests.Api.Presentation.Extensions;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddDatabase(configuration);

        services.AddOpenTelemetryTracing(configuration, environment);

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("api-version"));
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddOpenApi();

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials()
                    .WithExposedHeaders(
                        "Content-Type",
                        "Content-Length",
                        "X-Content-Type-Options",
                        "Cache-Control",
                        "Pragma",
                        "Expires")));

        var rateLimitConfig = configuration.GetSection("RateLimiting");
        var permitLimit = rateLimitConfig.GetValue("PermitLimit", 100);
        var windowSeconds = rateLimitConfig.GetValue("WindowSeconds", 60);

        services.AddRateLimiter(options =>
            options.AddFixedWindowLimiter("default", config =>
            {
                config.PermitLimit = permitLimit;
                config.Window = TimeSpan.FromSeconds(windowSeconds);
            }));

        services
            .AddKeycloakAuth(configuration, environment)
            .AddAuthorizationBuilder()
                .AddDefaultPolicy("AuthenticatedUsers", policy =>
                    policy.RequireAuthenticatedUser());

        services
            .AddApplicationServices()
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            o.SerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
        });

        services
            .AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            });

        services.AddHttpsRedirection(options => options.HttpsPort = 7289);

        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                "Database", tags: ["ready"])
            .AddCheck<KeycloakHealthCheck>(
                "Keycloak", tags: ["ready"]);

        return services;
    }
}

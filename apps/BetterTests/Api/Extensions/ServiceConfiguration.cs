using Asp.Versioning;

using BetterTests.Application.Mappings;
using BetterTests.Infrastructure.HealthChecks;

using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

using Microsoft.AspNetCore.RateLimiting;

using MicroElements.AspNetCore.OpenApi.FluentValidation;

namespace BetterTests.Api.Extensions;

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

        services.AddFluentValidationRulesToOpenApi();
        services.AddOpenApi("v1", options =>
        {
            options.AddFluentValidationRules();
        });

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .WithExposedHeaders("Content-Type")));

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
            .AddFluentValidationAutoValidation();

        services
            .AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            });

        services.AddHttpsRedirection(options => options.HttpsPort = 3001);

        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                "Database", tags: ["ready"])
            .AddCheck<KeycloakHealthCheck>(
                "Keycloak", tags: ["ready"]);

        services
            .AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(30);
                setup.MaximumHistoryEntriesPerEndpoint(50);
                setup.AddHealthCheckEndpoint("BetterTests API", "/healthz");
            })
            .AddInMemoryStorage();

        return services;
    }
}

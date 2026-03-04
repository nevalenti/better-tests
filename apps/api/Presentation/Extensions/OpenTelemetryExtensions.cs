using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BetterTests.Presentation.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var serviceName = "better-tests-api";
        var serviceVersion = "1.0.0";

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        { "environment", environment.EnvironmentName },
                        { "deployment.environment", environment.EnvironmentName }
                    }))
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext =>
                        {
                            return !httpContext.Request.Path.StartsWithSegments("/healthz");
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });

                builder.AddConsoleExporter();
            });

        return services;
    }
}

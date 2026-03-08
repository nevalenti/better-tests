using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BetterTests.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var serviceName = "better-tests-api";
        var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        var otelConfig = configuration.GetSection("OpenTelemetry");
        var exporterType = otelConfig["ExporterType"] ?? (environment.IsDevelopment() ? "console" : "none");

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        { "environment", environment.EnvironmentName },
                        { "deployment.environment", environment.EnvironmentName },
                        { "service.instance.id", Environment.MachineName }
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
                    })
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });

                ConfigureTraceExporter(builder, exporterType, otelConfig);
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation();

                ConfigureMetricsExporter(builder, exporterType, otelConfig);
            });

        return services;
    }

    private static void ConfigureTraceExporter(
        TracerProviderBuilder builder,
        string exporterType,
        IConfigurationSection otelConfig)
    {
        if (exporterType == "otlp")
        {
            var endpoint = otelConfig["OtlpEndpoint"] ?? "http://localhost:4317";
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(endpoint);
            });
        }
        else
        {
            builder.AddConsoleExporter();
        }
    }

    private static void ConfigureMetricsExporter(
        MeterProviderBuilder builder,
        string exporterType,
        IConfigurationSection otelConfig)
    {
        if (exporterType == "otlp")
        {
            var endpoint = otelConfig["OtlpEndpoint"] ?? "http://localhost:4317";
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(endpoint);
            });
        }
        else if (exporterType != "none")
        {
            builder.AddConsoleExporter();
        }
    }
}

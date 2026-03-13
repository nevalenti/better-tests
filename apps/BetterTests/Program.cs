using BetterTests.Api.Extensions;

using DotNetEnv;

using Serilog;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);
var appVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ApplicationVersion", appVersion)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

try
{
    builder.Host.UseSerilog();

    builder.Services.ConfigureApplicationServices(builder.Configuration, builder.Environment);

    var app = builder.Build();

    Log.Information(
        "Starting BetterTests API v{Version} in {Environment}",
        appVersion,
        app.Environment.EnvironmentName);

    app.ConfigureApplicationPipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

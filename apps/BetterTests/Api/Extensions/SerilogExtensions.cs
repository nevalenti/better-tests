using Serilog;
using Serilog.Events;

namespace BetterTests.Api.Extensions;

public static class SerilogExtensions
{
    public static WebApplication ConfigureSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = GetLogLevel;
            options.EnrichDiagnosticContext = EnrichWithUserId;
            options.MessageTemplate = "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        });

        return app;
    }

    private static LogEventLevel GetLogLevel(HttpContext httpContext, double elapsed, Exception? ex)
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= 400)
            return LogEventLevel.Warning;

        return LogEventLevel.Information;
    }

    private static void EnrichWithUserId(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("UserId",
            httpContext.User?.FindFirst("sub")?.Value ?? "anonymous");
    }
}

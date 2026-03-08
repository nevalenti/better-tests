using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BetterTests.Infrastructure.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var logLevel = GetLogLevel(statusCode);

            _logger.Log(
                logLevel,
                "{Method} {Path} → {StatusCode} ({ElapsedMs}ms)",
                context.Request.Method,
                context.Request.PathBase + context.Request.Path + context.Request.QueryString,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "{Method} {Path} → Exception ({ElapsedMs}ms)",
                context.Request.Method,
                context.Request.PathBase + context.Request.Path + context.Request.QueryString,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}

using Microsoft.AspNetCore.Http;

using Serilog.Context;

namespace BetterTests.Infrastructure.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private const string CorrelationIdProperty = "CorrelationId";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
            ? headerValue.ToString()
            : context.TraceIdentifier;

        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty(CorrelationIdProperty, correlationId))
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}

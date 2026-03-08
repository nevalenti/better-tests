using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BetterTests.Infrastructure.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers.XContentTypeOptions = "nosniff";

        headers.XFrameOptions = "DENY";

        headers.XXSSProtection = "1; mode=block";

        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        headers["Permissions-Policy"] =
            "geolocation=(), microphone=(), camera=(), magnetometer=(), gyroscope=(), accelerometer=()";

        var styleSrc = environment.IsDevelopment() ? "style-src 'self' 'unsafe-inline'" : "style-src 'self'";
        headers.ContentSecurityPolicy =
            $"default-src 'self'; script-src 'self'; {styleSrc}; img-src 'self' https:; font-src 'self'; connect-src 'self' https:";

        if (context.Request.IsHttps)
        {
            headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains";
        }

        await next(context);
    }
}

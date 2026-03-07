using Microsoft.AspNetCore.Http;

namespace BetterTests.Infrastructure.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers.XContentTypeOptions = "nosniff";

        headers["X-Frame-Options"] = "DENY";

        headers.XXSSProtection = "1; mode=block";

        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        headers["Permissions-Policy"] =
            "geolocation=(), microphone=(), camera=(), magnetometer=(), gyroscope=(), accelerometer=()";

        headers.ContentSecurityPolicy =
            "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' https:; font-src 'self'; connect-src 'self' https:";

        if (context.Request.IsHttps)
        {
            headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains";
        }

        await next(context);
    }
}

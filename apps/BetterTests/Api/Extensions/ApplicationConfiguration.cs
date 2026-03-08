using BetterTests.Infrastructure.Middleware;

using Scalar.AspNetCore;

using Serilog;

namespace BetterTests.Api.Extensions;

public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseHttpsRedirection();
        app.UseRateLimiter();
        app.UseCors();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/healthz").DisableRateLimiting();
        app.MapControllers().RequireAuthorization("AuthenticatedUsers");

        return app;
    }
}

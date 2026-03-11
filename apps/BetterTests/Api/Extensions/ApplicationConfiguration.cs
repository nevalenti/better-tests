using Scalar.AspNetCore;

namespace BetterTests.Api.Extensions;

public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        app.UseCorrelationId();
        app.UseExceptionHandling();
        app.ConfigureSerilogRequestLogging();

        app.UseSecurityHeaders();
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
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

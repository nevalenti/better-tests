using Scalar.AspNetCore;
using Serilog;

namespace BetterTests.Presentation.Extensions;

public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseExceptionHandling();
        app.UseSecurityHeaders();
        app.UseRateLimiter();
        app.UseCors();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/healthz");
        app.MapControllers().RequireAuthorization("AuthenticatedUsers");

        return app;
    }
}

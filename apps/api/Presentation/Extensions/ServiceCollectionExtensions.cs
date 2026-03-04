using BetterTests.Application.Interfaces;
using BetterTests.Application.Services;
using BetterTests.Application.Validators;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Persistence;
using FluentValidation;

namespace BetterTests.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddUnitOfWork();
        services.AddDomainServices();
        services.AddInputValidation();
        services.AddHealthCheckServices();

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITestSuiteService, TestSuiteService>();
        services.AddScoped<ITestCaseService, TestCaseService>();
        services.AddScoped<ITestCaseStepService, TestCaseStepService>();
        services.AddScoped<ITestRunService, TestRunService>();
        services.AddScoped<ITestResultService, TestResultService>();

        return services;
    }

    private static IServiceCollection AddInputValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateProjectRequestValidator>();
        return services;
    }

    private static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHttpClient<Infrastructure.HealthChecks.KeycloakHealthCheck>();
        return services;
    }
}

using BetterTests.Application.Interfaces;
using BetterTests.Application.Services;
using BetterTests.Application.Validators;
using BetterTests.Domain.Interfaces;
using BetterTests.Infrastructure.Repositories;

using FluentValidation;

namespace BetterTests.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddDomainServices();
        services.AddInputValidation();
        services.AddHealthCheckServices();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITestSuiteRepository, TestSuiteRepository>();
        services.AddScoped<ITestCaseRepository, TestCaseRepository>();
        services.AddScoped<ITestCaseStepRepository, TestCaseStepRepository>();
        services.AddScoped<ITestRunRepository, TestRunRepository>();
        services.AddScoped<ITestResultRepository, TestResultRepository>();

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

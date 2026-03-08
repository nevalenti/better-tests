using System.Text.Json;

using BetterTests.Domain.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BetterTests.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    private const string FallbackErrorResponse = """{"status":500,"title":"Internal Server Error"}""";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
            return Task.CompletedTask;

        context.Response.ContentType = "application/problem+json";
        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";

        var problemDetails = new ProblemDetailsResponse
        {
            Instance = context.Request.Path,
            StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
            InnerException = _environment.IsDevelopment() ? GetInnerException(exception.InnerException, maxDepth: 5) : null
        };

        switch (exception)
        {
            case EntityNotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Status = 404;
                problemDetails.Type = "https://api.bettertests.local/errors/entity-not-found";
                problemDetails.Title = "Entity Not Found";
                problemDetails.Detail = ex.Message;
                break;

            case DuplicateEntityException ex:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Status = 409;
                problemDetails.Type = "https://api.bettertests.local/errors/duplicate-entity";
                problemDetails.Title = "Duplicate Entity";
                problemDetails.Detail = ex.Message;
                break;

            case ParentNotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Status = 404;
                problemDetails.Type = "https://api.bettertests.local/errors/parent-not-found";
                problemDetails.Title = "Parent Entity Not Found";
                problemDetails.Detail = ex.Message;
                break;

            case FluentValidation.ValidationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = 400;
                problemDetails.Type = "https://api.bettertests.local/errors/validation-error";
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
                break;

            case BetterTests.Domain.Exceptions.ValidationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = 400;
                problemDetails.Type = "https://api.bettertests.local/errors/validation-error";
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = ex.Message;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = 500;
                problemDetails.Type = "https://api.bettertests.local/errors/internal-error";
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later.";
                break;
        }

        try
        {
            var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
            return context.Response.WriteAsync(json);
        }
        catch
        {
            return context.Response.WriteAsync(FallbackErrorResponse);
        }
    }

    private ProblemDetailsResponse? GetInnerException(Exception? exception, int maxDepth = 5, int currentDepth = 0)
    {
        if (exception == null || currentDepth >= maxDepth)
            return null;

        return new ProblemDetailsResponse
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            StackTrace = exception.StackTrace,
            InnerException = GetInnerException(exception.InnerException, maxDepth, currentDepth + 1)
        };
    }
}

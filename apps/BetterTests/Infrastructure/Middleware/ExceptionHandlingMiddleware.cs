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
            var (statusCode, _) = CategorizeException(ex);
            _logger.LogError(
                ex,
                "{ExceptionType} on {Method} {Path} → {StatusCode}: {Message}",
                ex.GetType().Name,
                context.Request.Method,
                context.Request.Path,
                statusCode,
                ex.Message);
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

        var (statusCode, problemDetails) = CategorizeException(exception);
        problemDetails.Instance = context.Request.Path;
        problemDetails.StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null;
        problemDetails.InnerException = _environment.IsDevelopment() ? GetInnerException(exception.InnerException, maxDepth: 5) : null;

        context.Response.StatusCode = statusCode;

        try
        {
            var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
            return context.Response.WriteAsync(json);
        }
        catch (Exception jsonEx)
        {
            _logger.LogError(jsonEx, "Failed to serialize error response");
            return context.Response.WriteAsync(FallbackErrorResponse);
        }
    }

    private static (int StatusCode, ProblemDetailsResponse Details) CategorizeException(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException ex => (StatusCodes.Status404NotFound, new ProblemDetailsResponse
            {
                Status = 404,
                Type = "https://api.bettertests.local/errors/entity-not-found",
                Title = "Entity Not Found",
                Detail = ex.Message
            }),
            DuplicateEntityException ex => (StatusCodes.Status409Conflict, new ProblemDetailsResponse
            {
                Status = 409,
                Type = "https://api.bettertests.local/errors/duplicate-entity",
                Title = "Duplicate Entity",
                Detail = ex.Message
            }),
            ParentNotFoundException ex => (StatusCodes.Status404NotFound, new ProblemDetailsResponse
            {
                Status = 404,
                Type = "https://api.bettertests.local/errors/parent-not-found",
                Title = "Parent Entity Not Found",
                Detail = ex.Message
            }),
            FluentValidation.ValidationException ex => (StatusCodes.Status400BadRequest, new ProblemDetailsResponse
            {
                Status = 400,
                Type = "https://api.bettertests.local/errors/validation-error",
                Title = "Validation Error",
                Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
            }),
            BetterTests.Domain.Exceptions.ValidationException ex => (StatusCodes.Status400BadRequest, new ProblemDetailsResponse
            {
                Status = 400,
                Type = "https://api.bettertests.local/errors/validation-error",
                Title = "Validation Error",
                Detail = ex.Message
            }),
            _ => (StatusCodes.Status500InternalServerError, new ProblemDetailsResponse
            {
                Status = 500,
                Type = "https://api.bettertests.local/errors/internal-error",
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later."
            })
        };
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

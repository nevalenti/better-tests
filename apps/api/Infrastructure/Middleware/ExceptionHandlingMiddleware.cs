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
            InnerException = _environment.IsDevelopment() ? GetInnerException(exception.InnerException) : null
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

            case ValidationException ex:
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

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        return context.Response.WriteAsync(json);
    }

    private ProblemDetailsResponse? GetInnerException(Exception? exception)
    {
        if (exception == null)
            return null;

        return new ProblemDetailsResponse
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            StackTrace = exception.StackTrace,
            InnerException = GetInnerException(exception.InnerException)
        };
    }
}

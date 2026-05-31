using System.Net;
using System.Text.Json;
using FluentValidation;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode >= (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail = exception.Message,
            errors = GetValidationErrors(exception),
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }

    private static (int StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            ValidationException => ((int)HttpStatusCode.BadRequest, "Validation Failed"),
            NotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
            UnauthorizedException => ((int)HttpStatusCode.Unauthorized, "Unauthorized"),
            ConflictException => ((int)HttpStatusCode.Conflict, "Conflict"),
            DomainException => ((int)HttpStatusCode.BadRequest, "Bad Request"),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error")
        };

    private static Dictionary<string, string[]>? GetValidationErrors(Exception exception)
    {
        if (exception is not ValidationException validationException)
            return null;

        return validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
    }
}

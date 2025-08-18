// ExceptionHandlingMiddleware.cs
using Minicommerce.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Minicommerce.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
    private readonly Microsoft.Extensions.Logging.ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, Microsoft.Extensions.Logging.ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(Microsoft.AspNetCore.Http.HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response = new
        {
            success = false,
            message = "An error occurred",
            errors = (object?)null
        };

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationEx.Errors
                };
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    success = false,
                    message = notFoundEx.Message,
                    errors = (object?)null
                };
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    success = false,
                    message = "Unauthorized access",
                    errors = (object?)null
                };
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    success = false,
                    message = argEx.Message,
                    errors = (object?)null
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    success = false,
                    message = "An internal server error occurred",
                    errors = (object?)null
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
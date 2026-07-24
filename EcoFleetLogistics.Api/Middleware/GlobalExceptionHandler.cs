
using System.Text.Json;
using EcoFleetLogistics.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EcoFleetLogistics.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statuscode, title, detail, type) = exception switch
        {
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "The request body could not be read or is malformed.",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            ),
            InvalidOperationException invalidOpEx => (
                StatusCodes.Status400BadRequest, 
                "Bad Request / Business Rule Violation", 
                invalidOpEx.Message, 
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
                ),
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest, 
                "Validation Error", "One or more validation failures occurred.",
                "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2"
                ),
            UnauthorizedAccessException unauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "exception.Message",
                "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
            ),
            _ => (
                StatusCodes.Status500InternalServerError, 
                "Internal Server Error", 
                "An unexpected error occurred on the server.",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                )
        };

       httpContext.Response.StatusCode = statuscode;
       httpContext.Response.ContentType = "application/problem+json";

       var problemDetails = new ProblemDetails
        {
            Status = statuscode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
           problemDetails.Extensions["errors"] = validationException.Errors;
        }
    
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
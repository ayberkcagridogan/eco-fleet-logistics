using EcoFleet.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcoFleet.Shared.Kernel.Middlewares
{
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

            var (statusCode, title, detail, errors) = exception switch
            {
                BaseException baseException => (
                    (int)baseException.StatusCode,
                    baseException.GetType().Name,
                    baseException.Message,
                    baseException.Errors
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "InternalServerError",
                    "An unexpected error occurred. Please try again later.",
                    null
            )};

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            if (errors != null)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId.ToString();
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;   
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EcoFleet.Shared.Kernel.Middlewares
{
    public class SecurityAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityAuditMiddleware> _logger;

        public SecurityAuditMiddleware(RequestDelegate next, ILogger<SecurityAuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                _logger.LogWarning("Unauthorized Access (401): {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                _logger.LogWarning("Forbidden Access (403): User '{UserId}' attempted to access restricted endpoint {Method} {Path}", 
                    userId, context.Request.Method, context.Request.Path);
            }
        }
    }
}
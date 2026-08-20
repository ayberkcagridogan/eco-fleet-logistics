using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace  EcoFleet.Shared.Kernel.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("🚀 [START] {RequestName} | Data: {@Request}", requestName, request);

        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next();

            timer.Stop();

            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500) 
            {
                _logger.LogWarning(
                    "⚠️ [PERFORMANCE WARNING] Long running request: {RequestName} ({ElapsedMilliseconds} ms)", 
                    requestName, elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("✅ [SUCCESS] {RequestName} | Duration: {ElapsedMs} ms", requestName, timer.ElapsedMilliseconds);
            }
            
            return response;
        }
        catch(Exception ex)
        {
            timer.Stop();
            _logger.LogError(ex, "💥 [FAIL] {RequestName} | Duration: {ElapsedMs} ms | Exception: {Message}", requestName, timer.ElapsedMilliseconds, ex.Message);
              throw;
        }
    }
}
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EcoFleetLogistics.Application.Common.Behaviors;

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

        _logger.LogInformation("Handling {RequestName} {@Request}", requestName, request);

        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next();

            timer.Stop();

            _logger.LogInformation(
                "Handled {RequestName} successfully in {ElapsedMilliseconds} ms",
                requestName,
                timer.ElapsedMilliseconds);
            
            return response;
        }
        catch(Exception ex)
        {
            timer.Stop();

            _logger.LogError(
              ex,
              "Requst {RequestName} faild after {ElapsedMilliseconds} ms",
              requestName,
              timer.ElapsedMilliseconds);
              throw;
        }
    }
}
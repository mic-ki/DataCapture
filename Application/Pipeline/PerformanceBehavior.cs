using System.Diagnostics;
using Application.Abstraction;
using Application.Abstraction.Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Pipeline;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserAccessor currentUserAccessor)
    {
        _logger = logger;
        _currentUserAccessor = currentUserAccessor;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch? timer = null;
        
        Interlocked.Increment(ref RequestCounter.ExecutionCount);
        if (RequestCounter.ExecutionCount > 3) timer = Stopwatch.StartNew();

        var response = await next().ConfigureAwait(false);

        timer?.Stop();
        var elapsedMilliseconds = timer?.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserAccessor.SessionInfo?.UserName;

            _logger.LogWarning(
    "Long-running request detected: {RequestName} ({ElapsedMilliseconds}ms) {@Request} by {UserName}",
    requestName, elapsedMilliseconds, request, userName);
        }

        return response;
    }
}


public static class RequestCounter
{
    public static int ExecutionCount;
}
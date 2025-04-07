using Application.Abstraction.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _notificationPublisher;

    public Mediator(IServiceProvider serviceProvider, INotificationPublisher notificationPublisher)
    {
        _serviceProvider = serviceProvider;
        _notificationPublisher = notificationPublisher;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        var behaviorsType = typeof(IEnumerable<>).MakeGenericType(typeof(IPipelineBehavior<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse)));
        var behaviors = (IEnumerable<dynamic>)_serviceProvider.GetService(behaviorsType) ?? Enumerable.Empty<dynamic>();

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle((dynamic)request, cancellationToken);

        foreach (var behavior in behaviors.Reverse())
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle((dynamic)request, cancellationToken, next);
        }

        return await handlerDelegate();
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        if (!handlers.Any()) return;

        await _notificationPublisher.Publish(handlers, notification, cancellationToken);
    }
}

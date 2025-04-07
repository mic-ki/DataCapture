using Application.Abstraction.Mediator;

namespace Infrastructure.Mediator;

public class ParallelNotificationPublisher : INotificationPublisher
{
    public Task Publish<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));
        return Task.WhenAll(tasks);
    }
}
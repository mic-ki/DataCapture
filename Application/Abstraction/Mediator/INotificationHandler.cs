namespace Application.Abstraction.Mediator;

public interface INotificationHandler<TNotification>
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
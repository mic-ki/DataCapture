using Application.Abstraction.Mediator;
using Domain;
using Domain.Core;
using Microsoft.Extensions.Logging;

namespace Application.Features.Generic.EventHandlers;

public class EntityEventHandler<T, TEnum> : INotificationHandler<Event<T,TEnum>> where TEnum : Enum where T : class
{
    private readonly ILogger<EntityEventHandler<T, TEnum>> _logger;

    public EntityEventHandler(
        ILogger<EntityEventHandler<T, TEnum>> logger
    )
    {
        _logger = logger;
    }
    public Task Handle(Event<T, TEnum> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ", notification.GetType().Name, notification);
        return Task.CompletedTask;
    }
}
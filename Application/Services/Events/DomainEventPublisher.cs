using Application.Abstraction.Mediator;
using Domain;
using Domain.Core;

namespace Application.Services.Events;

public class DomainEventPublisher
{
    private readonly IMediator _mediator;

    public DomainEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        // Obalíme doménovou událost do notifikace pro MediatR
        var notification = CreateNotification(domainEvent);
        await _mediator.Publish(notification);
        domainEvent.MarkAsPublished();
    }

    private static INotification CreateNotification(DomainEvent domainEvent)
    {
        // Dynamicky vytvoří DomainEventNotification<T> pro konkrétní událost
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }

    public async Task Publish(IEnumerable<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await Publish(domainEvent);
        }
    }
}
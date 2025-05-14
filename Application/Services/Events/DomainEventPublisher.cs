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

        // TODO: Zde je problém - notifikace je typu DomainEventNotification<TestDomainEvent>,
        // ale voláme generickou metodu Publish<INotification>, která hledá handlery typu
        // INotificationHandler<INotification>, nikoliv INotificationHandler<DomainEventNotification<TestDomainEvent>>
        // chce to vymyslet něco bez reflexe

        // Použijeme reflexi pro volání správné generické metody
        var notificationType = notification.GetType();
        var publishMethod = typeof(IMediator).GetMethod(nameof(IMediator.Publish));

        if (publishMethod != null)
        {
            var genericPublishMethod = publishMethod.MakeGenericMethod(notificationType);
            var task = genericPublishMethod.Invoke(_mediator, new object[] { notification, CancellationToken.None });
            if (task is Task taskResult)
            {
                await taskResult;
            }
        }
        else
        {
            throw new InvalidOperationException($"Could not find Publish method on IMediator interface");
        }
        domainEvent.MarkAsPublished();
    }

    private static INotification CreateNotification(DomainEvent domainEvent)
    {
        // Dynamicky vytvoří DomainEventNotification<T> pro konkrétní událost
        var domainEventType = domainEvent.GetType();
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEventType);

        // Vytvoříme instanci notifikace s doménovou událostí
        var notification = Activator.CreateInstance(notificationType, domainEvent);

        if (notification == null)
        {
            throw new InvalidOperationException($"Failed to create notification for domain event of type {domainEventType.Name}");
        }

        return (INotification)notification;
    }

    public async Task Publish(IEnumerable<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await Publish(domainEvent);
        }
    }

    // Pomocná metoda pro získání správné generické metody
    private async Task PublishNotification(INotification notification)
    {
        var notificationType = notification.GetType();
        var publishMethod = typeof(IMediator).GetMethod(nameof(IMediator.Publish));

        if (publishMethod != null)
        {
            var genericPublishMethod = publishMethod.MakeGenericMethod(notificationType);
            var task = genericPublishMethod.Invoke(_mediator, new object[] { notification, CancellationToken.None });
            if (task is Task taskResult)
            {
                await taskResult;
            }
        }
        else
        {
            throw new InvalidOperationException($"Could not find Publish method on IMediator interface");
        }
    }
}
using Application.Abstraction.Mediator;
using Domain;
using Domain.Core;

namespace Application.Services.Events;

public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : DomainEvent  // Doménová událost z Domain Layer
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
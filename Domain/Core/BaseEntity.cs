using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Core;

public abstract class BaseEntity<T> : IEntity<T> 
{
    private readonly List<DomainEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual T Id { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

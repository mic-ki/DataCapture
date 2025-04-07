
namespace Domain.Core;

public abstract class DomainEvent 
{
    protected DomainEvent()
    {
        DateOccurred = DateTime.UtcNow;
    }
    public bool IsPublished { get; private set; }
    public DateTime DateOccurred { get; protected set; }

    public void MarkAsPublished()
    {
        IsPublished = true;
    }
}

public sealed class Event<T, TEnum>(T entity, TEnum eventType) : DomainEvent
    where TEnum : Enum
{
    public T Entity { get; } = entity;
    public TEnum EventType { get; } = eventType;
}

public enum EventType
{
    Created,
    Updated,
    Deleted
}
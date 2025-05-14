namespace Domain.Core;

/// <summary>
/// Abstraktní základní třída pro všechny doménové události.
/// Doménové události reprezentují významné změny v doméně, které mohou vyžadovat reakci jiných částí systému.
/// </summary>
public abstract class DomainEvent 
{
    /// <summary>
    /// Konstruktor nastavuje datum a čas vzniku události na aktuální UTC čas.
    /// </summary>
    /// TODO: Změna na DateTimeOffset (v Azure budou různá časová pásma) 
    protected DomainEvent()
    {
        DateOccurred = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Indikuje, zda byla událost již publikována a zpracována.
    /// </summary>
    public bool IsPublished { get; private set; }
    
    /// <summary>
    /// Datum a čas, kdy událost nastala.
    /// </summary>
    public DateTime DateOccurred { get; protected set; }

    /// <summary>
    /// Označí událost jako publikovanou.
    /// </summary>
    public void MarkAsPublished()
    {
        IsPublished = true;
    }
}

/// <summary>
/// Generická implementace doménové události, která může být použita pro různé typy entit a událostí.
/// </summary>
/// <typeparam name="T">Typ entity, ke které se událost vztahuje</typeparam>
/// <typeparam name="TEnum">Typ výčtu definující možné typy událostí</typeparam>
/// <param name="entity">Entita, ke které se událost vztahuje</param>
/// <param name="eventType">Typ události</param>
public sealed class Event<T, TEnum>(T entity, TEnum eventType) : DomainEvent
    where TEnum : Enum
{
    /// <summary>
    /// Entita, ke které se událost vztahuje.
    /// </summary>
    public T Entity { get; } = entity;
    
    /// <summary>
    /// Typ události.
    /// </summary>
    public TEnum EventType { get; } = eventType;
}

/// <summary>
/// Základní výčet typů událostí, které mohou nastat v systému.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Entita byla vytvořena.
    /// </summary>
    Created,
    
    /// <summary>
    /// Entita byla aktualizována.
    /// </summary>
    Updated,
    
    /// <summary>
    /// Entita byla smazána.
    /// </summary>
    Deleted
}

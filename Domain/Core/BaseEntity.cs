using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Core;

/// <summary>
/// Základní abstraktní třída pro všechny entity v doméně.
/// Poskytuje základní funkcionalitu pro identifikaci entity a práci s doménovými událostmi.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public abstract class BaseEntity<T> : IEntity<T> 
{
    /// <summary>
    /// Privátní kolekce doménových událostí spojených s touto entitou.
    /// </summary>
    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// Veřejná kolekce doménových událostí pro čtení.
    /// Atribut NotMapped zajišťuje, že tato vlastnost nebude mapována do databáze.
    /// </summary>
    [NotMapped] public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Jedinečný identifikátor entity.
    /// </summary>
    public virtual required T Id { get; set; }
    
    /// <summary>
    /// Verze řádku pro optimistické zamykání.
    /// Atribut Timestamp zajišťuje, že tato hodnota bude automaticky aktualizována při každé změně entity.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; }

    /// <summary>
    /// Přidá novou doménovou událost do kolekce událostí entity.
    /// </summary>
    /// <param name="domainEvent">Doménová událost k přidání</param>
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Odstraní doménovou událost z kolekce událostí entity.
    /// </summary>
    /// <param name="domainEvent">Doménová událost k odstranění</param>
    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Vyčistí všechny doménové události z kolekce.
    /// Typicky voláno po zpracování všech událostí.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

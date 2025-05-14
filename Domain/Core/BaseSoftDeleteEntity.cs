namespace Domain.Core;

/// <summary>
/// Abstraktní třída pro entity podporující měkké mazání (soft delete).
/// Rozšiřuje základní entitu o vlastnosti pro sledování smazání.
/// Měkké mazání umožňuje označit entitu jako smazanou bez fyzického odstranění z databáze.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public abstract class BaseSoftDeleteEntity<T> : BaseEntity<T>, ISoftDelete
{
    /// <summary>
    /// Datum a čas smazání entity.
    /// Null hodnota znamená, že entita nebyla smazána.
    /// </summary>
    public DateTime? Deleted { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu smazal.
    /// </summary>
    public string? DeletedBy { get; set; }
}
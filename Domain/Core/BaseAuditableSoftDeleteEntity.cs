namespace Domain.Core;

/// <summary>
/// Abstraktní třída kombinující auditování a měkké mazání.
/// Poskytuje vlastnosti pro sledování vytvoření, modifikace i smazání entity.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public abstract class BaseAuditableSoftDeleteEntity<T> : BaseAuditableEntity<T>, ISoftDelete
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
namespace Domain.Core;

/// <summary>
/// Abstraktní třída pro entity, které vyžadují auditování.
/// Rozšiřuje základní entitu o vlastnosti pro sledování vytvoření a poslední modifikace.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public abstract class BaseAuditableEntity<T> : BaseEntity<T>, IAuditableEntity<T>
{
    /// <summary>
    /// Datum a čas vytvoření entity.
    /// </summary>
    public DateTime? Created { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu vytvořil.
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Datum a čas poslední modifikace entity.
    /// </summary>
    public DateTime? LastModified { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu naposledy modifikoval.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
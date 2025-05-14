using System.ComponentModel.DataAnnotations;

namespace Domain.Core;

/// <summary>
/// Základní rozhraní pro všechny entity v doméně.
/// Slouží jako marker rozhraní pro identifikaci entit.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Rozšířené rozhraní pro entity s identifikátorem.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public interface IEntity<T> : IEntity
{
    /// <summary>
    /// Jedinečný identifikátor entity.
    /// </summary>
    T Id { get; set; }
    
    /// <summary>
    /// Verze řádku pro optimistické zamykání.
    /// Atribut Timestamp zajišťuje, že tato hodnota bude automaticky aktualizována při každé změně entity.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

/// <summary>
/// Rozhraní pro entity, které vyžadují auditování.
/// Definuje vlastnosti pro sledování vytvoření a poslední modifikace.
/// </summary>
/// <typeparam name="T">Typ identifikátoru entity (např. int, Guid, string)</typeparam>
public interface IAuditableEntity<T> : IEntity<T>
{
    /// <summary>
    /// Datum a čas vytvoření entity.
    /// </summary>
    DateTime? Created { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu vytvořil.
    /// </summary>
    string? CreatedBy { get; set; }
    
    /// <summary>
    /// Datum a čas poslední modifikace entity.
    /// </summary>
    DateTime? LastModified { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu naposledy modifikoval.
    /// </summary>
    string? LastModifiedBy { get; set; }
}

/// <summary>
/// Rozhraní pro entity podporující měkké mazání (soft delete).
/// Definuje vlastnosti pro sledování smazání.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Datum a čas smazání entity.
    /// Null hodnota znamená, že entita nebyla smazána.
    /// </summary>
    DateTime? Deleted { get; set; }
    
    /// <summary>
    /// Identifikátor uživatele, který entitu smazal.
    /// </summary>
    string? DeletedBy { get; set; }
}

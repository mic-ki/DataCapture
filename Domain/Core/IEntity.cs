using System.ComponentModel.DataAnnotations;

namespace Domain.Core;

public interface IEntity
{
}

public interface IEntity<T> : IEntity
{
    T Id { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

public interface IAuditableEntity<T> : IEntity<T>
{
    DateTime? Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }
    string? DeletedBy { get; set; }
}
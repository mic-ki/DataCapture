namespace Domain.Core;

public abstract class BaseAuditableEntity<T> : BaseEntity<T>, IAuditableEntity<T>
{
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
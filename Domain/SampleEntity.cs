using Domain.Core;

namespace Domain;

public class SampleEntity: BaseAuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Age { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}
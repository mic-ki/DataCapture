using Domain.Core;

namespace Domain;

public class SampleEntity: BaseAuditableEntity<int>
{
    public string? Name { get; set; }
}
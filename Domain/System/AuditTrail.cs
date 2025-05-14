using Domain.Core;

namespace Domain.System;

public class AuditTrail : BaseEntity<int>
{
    public string? UserId { get; set; }
    public string? TableName { get; set; }
    public DateTime DateTime { get; set; }
    public Dictionary<string, object?>? OldValues { get; set; }
    public Dictionary<string, object?>? NewValues { get; set; }
    public List<string>? AffectedColumns { get; set; }
    public Dictionary<string, object> PrimaryKey { get; set; } = new();
    public string? DebugView { get; set; }
    public string? ErrorMessage { get; set; }
}
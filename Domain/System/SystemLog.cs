using Domain.Core;

namespace Domain.System;

public class SystemLog : BaseEntity<int>
{
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public string Level { get; set; } = null!;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public string? Exception { get; set; }
    public string? UserName { get; set; }
    public string? ClientIP { get; set; }
    public string? ClientAgent { get; set; }
    public string? Properties { get; set; }
    public string? LogEvent { get; set; }
}
namespace Application.Models;

public class SessionInfo
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; } 
    public string? IpAddress { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public string? Status { get; set; } // Např. "Active", "PendingApproval"
    public DateTime SessionStartTime { get; set; }
    public bool IsAuthenticated { get; set; }
}
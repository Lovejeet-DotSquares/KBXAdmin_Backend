namespace KBXAdmin.Domain.Entities;

public class LoginAuditLog
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string? Email { get; set; }

    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutTime { get; set; }

    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

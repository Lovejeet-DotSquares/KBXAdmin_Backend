using KBXAdmin.Domain.Common;

public class User : BaseEntity
{
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public string Role { get; set; } = "User"; // Admin / User / Reviewer
    public bool IsActive { get; set; } = true;
}

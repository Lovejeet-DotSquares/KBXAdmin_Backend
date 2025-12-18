using KBXAdmin.Domain.Enums;

public class RegisterRequestDto
{
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.User;
}

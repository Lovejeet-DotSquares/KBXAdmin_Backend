using KBXAdmin.Application.DTOs;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent);
    Task LogoutAsync(Guid loginLogId);
    Task<UserDto> RegisterAsync(RegisterRequestDto request);
}

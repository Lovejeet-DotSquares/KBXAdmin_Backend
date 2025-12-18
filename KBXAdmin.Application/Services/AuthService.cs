using KBXAdmin.Application.DTOs;
using KBXAdmin.Common.Security;
using KBXAdmin.Domain.Entities;
using KBXAdmin.Infrastructure.Repositories.Interfaces;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ILoginAuditLogRepository _logRepo;
    private readonly JwtTokenGenerator _jwt;

    public AuthService(IUserRepository userRepo, ILoginAuditLogRepository logRepo, JwtTokenGenerator jwt)
    {
        _userRepo = userRepo;
        _logRepo = logRepo;
        _jwt = jwt;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ip, string userAgent)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            await LogAttemptAsync(null, request.Email, false, "Invalid credentials", ip, userAgent);
            throw new Exception("Invalid credentials");
        }

        var (token, expiresAt) = _jwt.Generate(user);

        var logId = await LogAttemptAsync(user, user.Email, true, null, ip, userAgent);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            LoginAuditLogId = logId
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null) throw new Exception("Email already registered");

        PasswordHasher.CreatePasswordHash(request.Password, out var hash, out var salt);

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = request.Role.ToString(),
            IsActive = true
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Role = user.Role,
            IsActive = user.IsActive
        };
    }

    public async Task LogoutAsync(Guid logId)
    {
        var log = await _logRepo.GetByIdAsync(logId);
        if (log != null)
        {
            log.LogoutTime = DateTime.UtcNow;
            _logRepo.Update(log);
            await _logRepo.SaveChangesAsync();
        }
    }

    private async Task<long> LogAttemptAsync(User? user, string? email, bool success, string? reason, string ip, string userAgent)
    {
        var log = new LoginAuditLog
        {
            UserId = user?.Id,
            Email = email,
            IsSuccessful = success,
            FailureReason = reason,
            IpAddress = ip,
            UserAgent = userAgent,
            LoginTime = DateTime.UtcNow
        };

        await _logRepo.AddAsync(log);
        await _logRepo.SaveChangesAsync();
        return log.Id;
    }
}

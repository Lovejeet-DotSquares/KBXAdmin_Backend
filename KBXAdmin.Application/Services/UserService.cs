using KBXAdmin.Application.DTOs;
using KBXAdmin.Application.Interfaces;
using KBXAdmin.Domain.Enums;
using KBXAdmin.Infrastructure.Repositories.Interfaces;

namespace KBXAdmin.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // -------------------------------------------------------
    // GET ALL USERS
    // -------------------------------------------------------
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            UserName = u.UserName,
            Role = u.Role,
            IsActive = u.IsActive
        });
    }

    // -------------------------------------------------------
    // CHANGE USER ROLE
    // -------------------------------------------------------
    public async Task ChangeUserRoleAsync(Guid userId, UserRole newRole)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        if (!Enum.IsDefined(typeof(UserRole), newRole))
            throw new Exception("Invalid role");

        user.Role = newRole.ToString();

        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
    }

    // -------------------------------------------------------
    // DEACTIVATE USER
    // -------------------------------------------------------
    public async Task DeactivateUserAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        if (!user.IsActive)
            throw new Exception("User is already inactive");

        user.IsActive = false;

        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
    }
}

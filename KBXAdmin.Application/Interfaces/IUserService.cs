using KBXAdmin.Application.DTOs;
using KBXAdmin.Domain.Enums;

namespace KBXAdmin.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task ChangeUserRoleAsync(Guid userId, UserRole newRole);
    Task DeactivateUserAsync(Guid userId);
}

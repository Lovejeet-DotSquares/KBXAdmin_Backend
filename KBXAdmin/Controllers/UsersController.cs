using KBXAdmin.Application.DTOs;
using KBXAdmin.Application.Services;
using KBXAdmin.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KBXAdmin.Application.Interfaces;

namespace KBXAdmin.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]   // Only admins manage users
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // PUT: api/users/{id}/role?role=Admin
    [HttpPut("{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid userId, [FromQuery] UserRole role)
    {
        await _userService.ChangeUserRoleAsync(userId, role);
        return NoContent();
    }

    // PUT: api/users/{id}/deactivate
    [HttpPut("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid userId)
    {
        await _userService.DeactivateUserAsync(userId);
        return NoContent();
    }
}

using System.Security.Claims;
using IncidentManagement.Application.Dtos.Requests;
using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class UserController(IncidentDbContext db) : ControllerBase
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        if (!await IsActiveAdministrator()) return Unauthorized();
        return Ok(await db.Users.AsNoTracking().Include(x => x.Role).Select(x => new UserResponse(
            x.UserId, x.Username ?? string.Empty, x.Email ?? string.Empty,
            x.PhoneNumber ?? string.Empty, x.WhatsAppNumber ?? string.Empty,
            x.PropertyId, x.IsActive, x.Role != null ? x.Role.RoleName ?? "User" : "User"))
            .ToListAsync());
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserApiRequest request)
    {
        if (!await IsActiveAdministrator()) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Trim().All(c => c == '0') ||
            string.IsNullOrWhiteSpace(request.Email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return BadRequest(new { message = "A valid username, email, and password of at least 8 characters are required." });
        if (!await db.Properties.AnyAsync(x => x.PropertyId == request.PropertyId))
            return BadRequest(new { message = "Select a valid property." });
        if (await db.Users.AnyAsync(x => x.Email == request.Email.Trim() || x.Username == request.Username.Trim()))
            return Conflict(new { message = "Email or username already exists." });
        var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Resident");
        if (role is null) return Problem("The Resident role is missing from dbo.Roles.");
        var hash = _passwordHasher.HashPassword(null!, request.Password);
        var user = new User(request.Username.Trim(), request.Email.Trim(), request.PhoneNumber ?? string.Empty,
            request.WhatsAppNumber ?? string.Empty, hash, Array.Empty<byte>(), request.PropertyId, role.RoleId);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Ok(new { user.UserId, message = "User created successfully." });
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateUserStatusRequest request)
    {
        if (!await IsActiveAdministrator()) return Unauthorized();
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (request.UserId == currentUserId && !request.IsActive)
            return BadRequest(new { message = "You cannot deactivate your own administrator account." });
        var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (user is null) return NotFound(new { message = "User not found." });
        user.SetActive(request.IsActive);
        await db.SaveChangesAsync();
        return Ok(new { message = "User status updated successfully." });
    }

    private async Task<bool> IsActiveAdministrator()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return false;
        return await db.Users.AnyAsync(u => u.UserId == id && u.IsActive &&
            u.Role != null && u.Role.RoleName == "Admin");
    }
}

public sealed record CreateUserApiRequest(string Username, string Email, string Password,
    string? PhoneNumber, string? WhatsAppNumber, int PropertyId);

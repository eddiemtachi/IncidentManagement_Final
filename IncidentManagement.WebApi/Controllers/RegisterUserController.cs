using System.Net.Mail;
using IncidentManagement.Application.Dtos.Requests;
using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RegisterUserController : ControllerBase
{
    private readonly IncidentDbContext _db;
    private readonly PasswordHasher<object> _passwordHasher = new();

    public RegisterUserController(IncidentDbContext db) => _db = db;

    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserRequest request)
    {
        var username = request.Username?.Trim() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username) || username.All(c => c == '0'))
            return BadRequest(new { message = "Username is required and cannot consist only of zeros." });
        try
        {
            var parsed = new MailAddress(email);
            if (!string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase) || !email.Contains('.'))
                return BadRequest(new { message = "Enter a valid email address." });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Enter a valid email address." });
        }
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return BadRequest(new { message = "Password must contain at least 8 characters." });
        if (request.PropertyId <= 0 || !await _db.Properties.AnyAsync(p => p.PropertyId == request.PropertyId))
            return BadRequest(new { message = "Select a valid property." });
        if (await _db.Users.AnyAsync(u => u.Email == email || u.Username == username))
            return Conflict(new { message = "That email or username is already registered." });

        var defaultRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Resident");
        if (defaultRole is null)
            return Problem("The Resident role is missing from dbo.Roles. Insert it before registering users.");

        var hash = _passwordHasher.HashPassword(null!, request.Password);
        var user = new User(username, email, request.PhoneNumber?.Trim() ?? string.Empty,
            request.WhatsAppNumber?.Trim() ?? string.Empty, hash, Array.Empty<byte>(),
            request.PropertyId, defaultRole.RoleId);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new RegisterUserResponse(user.UserId, user.Username ?? string.Empty,
            user.Email ?? string.Empty, "User registered successfully."));
    }
}

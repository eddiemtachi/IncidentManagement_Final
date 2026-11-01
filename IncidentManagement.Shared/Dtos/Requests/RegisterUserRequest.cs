using System.ComponentModel.DataAnnotations;

namespace IncidentManagement.Application.Dtos.Requests
{
    public record RegisterUserRequest(
        [Required, StringLength(50, MinimumLength = 3)] string Username,
        [Required, EmailAddress] string Email,
        [Required, MinLength(8)] string Password,
        string? PhoneNumber,
        string? WhatsAppNumber,
        int PropertyId);
}

namespace IncidentManagement.Application.Dtos.Requests
{
    public record CreateUserRequest(string Username, string Email, string PhoneNumber, string WhatsAppNumber, byte[] PasswordSalt, string PasswordHash, int PropertyId);
}

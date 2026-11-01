namespace IncidentManagement.Application.Dtos.Responses
{
    public record RegisterUserResponse(int UserId, string Username, string Email, string Message);
}

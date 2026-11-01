using IncidentManagement.Application.Dtos.Responses;

namespace IncidentManagement.WebClient.Interfaces
{
    public interface IIncidentService
    {
        Task<List<IncidentResponse>> FetchAllIncidentsAsync();
    }
}
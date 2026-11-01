using IncidentManagement.Application.Dtos.Requests;
using IncidentManagement.Application.Dtos.Responses;

namespace IncidentManagement.WebClient.Services
{
    public class IncidentApiClientRepository
    {
        private readonly ApiClient _client;

        public IncidentApiClientRepository(ApiClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<IncidentResponse>> GetAllIncidentsAsync()
        {
            return _client.GetAsync<IEnumerable<IncidentResponse>>("api/Incident/all");
        }

        public Task<IEnumerable<IncidentResponse>> GetByPropertyAsync(int propertyId) =>
            _client.GetAsync<IEnumerable<IncidentResponse>>($"api/Incident/property/{propertyId}");

        public Task<IncidentResponse> CreateIncidentAsync(CreateIncidentRequest request)
        {
            return _client.PostAsync<IncidentResponse>("api/Incident/create", request);
        }

        public Task<IncidentResponse> UpdateIncidentStatusAsync(UpdateIncidentStatusRequest request) =>
            _client.PutAsync<IncidentResponse>("api/Incident/status", request);
    }
}

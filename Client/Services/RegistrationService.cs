using System.Net.Http.Json;
using SupportSystemCofe.Shared.Models;

namespace SupportSystemCofe.Client.Services
{
    public class RegistrationService
    {
        private readonly HttpClient _httpClient;

        public RegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterAsync(RegistrationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/registration", request);
            return response.IsSuccessStatusCode;
        }
    }
}

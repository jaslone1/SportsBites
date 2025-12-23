// GameDayParty.Client/Services/EventService.cs

using GameDayParty.Shared;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameDayParty.Client.Services
{
    public class EventService
    {
        private readonly HttpClient _httpClient;

        public EventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventDto>?> GetEventsAsync()
        {
            // API endpoint that will return all events
            return await _httpClient.GetFromJsonAsync<List<EventDto>>("api/events");
        }
    }
}
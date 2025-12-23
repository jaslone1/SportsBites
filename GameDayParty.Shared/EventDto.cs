// GameDayParty.Shared/EventDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace GameDayParty.Shared
{
    public class EventDto
    {
        public int EventId { get; set; }
        [Required(ErrorMessage = "Event Name is required.")]
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; } = DateTime.Now.Date.AddDays(7);
        // Game Details (Combined string for simplicity in the DTO)
        public string GameDetails { get; set; } = string.Empty; // Name of game(s) that are viewed or party reason
        // Host Information
        public int HostUserId { get; set; }
        public string HostName { get; set; } = string.Empty;
        public bool IsFinalized { get; set; }
        public List<FoodSuggestionDto> FoodSuggestions { get; set; } = new();
    }
}
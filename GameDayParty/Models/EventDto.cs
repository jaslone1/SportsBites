namespace GameDayParty.Models
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string GameDetails { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string? HostUserId { get; set; }
        public bool IsFinalized { get; set; }
        public List<FoodSuggestionDto> FoodSuggestions { get; set; } = new();
    }
}
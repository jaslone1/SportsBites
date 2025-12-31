namespace GameDayParty.Models
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string GameDetails { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string HostUserId { get; set; } = string.Empty;
        public bool IsFinalized { get; set; }
        public List<FoodSuggestionDto> FoodSuggestions { get; set; } = new();
    }

    public class FoodSuggestionDto
    {
        public int FoodSuggestionId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public string SuggestedByName { get; set; } = string.Empty;
        public int UpvoteCount { get; set; }
        public bool HasUserUpvoted { get; set; }
        public string? ClaimedByName { get; set; }
    }
}
namespace GameDayParty.Models
{
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
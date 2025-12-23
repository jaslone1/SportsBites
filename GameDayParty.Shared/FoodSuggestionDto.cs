// GameDayParty.Shared/FoodSuggestionDto.cs
namespace GameDayParty.Shared
{
    public class FoodSuggestionDto
    {
        public int FoodSuggestionId { get; set; }
        public int EventId { get; set; }
        
        // Food Information
        public string FoodName { get; set; } = string.Empty;
        public string FoodCategory { get; set; } = string.Empty;
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }

        // Suggested By
        public string SuggestedByName { get; set; } = string.Empty;

        // Current Vote Count - Probably remove downvotes
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }

        // Vote Tracking
        public bool HasUserUpvoted { get; set; } 
        public bool HasUserDownvoted { get; set; } 
        
    }
}
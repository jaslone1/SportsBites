using System.ComponentModel.DataAnnotations;

namespace GameDayParty.Models
{
    public class FoodSuggestion
    {
        [Key]
        public int FoodSuggestionId { get; set; }
        public int EventId { get; set; }
        [Required]
        public string FoodName { get; set; } = string.Empty;
        public string SuggestedByName { get; set; } = string.Empty;
        public Event? Event {get; set; }
        public string FoodCategory { get; set; } = string.Empty;
        public bool IsVegetarian { get; set; }
        public int UpvoteCount { get; set; } = 0;
        public string? ClaimedByName { get; set; }
    }
}
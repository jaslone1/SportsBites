using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDayParty.Models
{
    public class UserVote
    {
        public int UserVoteId { get; set; }
        public int FoodSuggestionId { get; set; }
        public string VoterName { get; set; } = string.Empty; 
        // Navigation property
        public FoodSuggestion FoodSuggestion { get; set; } = null!;
    }
}
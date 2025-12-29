using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace GameDayParty.Models

{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string GameDetails { get; set; } = string.Empty;
        public int HostUserId { get; set; }
        public string HostName { get; set; } = string.Empty;
        public bool IsFinalized { get; set; }
        public List<FoodSuggestion> FoodSuggestions { get; set; } = new();
    }
}
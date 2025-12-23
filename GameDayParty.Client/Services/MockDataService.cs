// GameDayParty.Client/Services/MockDataService.cs
using GameDayParty.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GameDayParty.Client.Services
{
    public class MockDataService
    {
        public EventDto GetEvent(int eventId) => new EventDto
        {
            EventId = eventId,
            EventName = "Sunday Night Football Fiesta",
            HostName = "Jared Slone",
            GameDetails = "Packers vs. Vikings (NFC North)",
            IsFinalized = false,
            EventDate = DateTime.Now.AddDays(7)
        };

        public List<FoodSuggestionDto> GetSuggestions(int eventId) => new List<FoodSuggestionDto>
        {
            new FoodSuggestionDto { 
                FoodSuggestionId = 1, 
                EventId = 1,
                FoodName = "Buffalo Chicken Dip", 
                FoodCategory = "Appetizer", 
                UpvoteCount = 12, 
                DownvoteCount = 2,
                SuggestedByName = "Alex",
                HasUserUpvoted = true // Mock current user vote
            },
            new FoodSuggestionDto { 
                FoodSuggestionId = 2,
                EventId = 1,
                FoodName = "Black Bean Sliders", 
                FoodCategory = "Entree", 
                IsVegetarian = true,
                UpvoteCount = 8, 
                DownvoteCount = 1,
                SuggestedByName = "Sam",
                HasUserDownvoted = false
            },
            new FoodSuggestionDto { 
                FoodSuggestionId = 3, 
                EventId = 1,
                FoodName = "Taco Bar", 
                FoodCategory = "Entree", 
                UpvoteCount = 2, 
                DownvoteCount = 15,
                SuggestedByName = "Chris"
            }
        }.Where(s => s.EventId == eventId).ToList();
    }
}
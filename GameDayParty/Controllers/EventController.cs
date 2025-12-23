// GameDayParty/Controllers/EventsController.cs

using GameDayParty.Data;
using GameDayParty.Models;
using GameDayParty.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This defines the API route: /api/events
[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        // Fetch all events from the database
        var events = await _context.Events
            .Select(e => new EventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDate = e.EventDate,
                GameDetails = e.GameDetails,
                HostName = e.HostName,
                HostUserId = e.HostUserId,
                IsFinalized = e.IsFinalized
            })
            .ToListAsync();

        return events;
    }

    // GET: api/Events/#
    [HttpGet("{eventId}")]
    public async Task<ActionResult<EventDto>> GetEvent(int eventId, [FromQuery] string? voterName = null)
    {
        var eventModel = await _context.Events
            .Include(e => e.FoodSuggestions) 
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventModel == null) return NotFound();
        
        var userVotes = new List<int>();
        if (!string.IsNullOrEmpty(voterName))
        {
            userVotes = await _context.UserVotes
                .Where(v => v.VoterName == voterName)
                .Select(v => v.FoodSuggestionId)
                .ToListAsync();
        }

        // Map Model to DTO for response
        var eventDto = new EventDto
        {
            EventId = eventModel.EventId,
            EventName = eventModel.EventName,
            EventDate = eventModel.EventDate,
            GameDetails = eventModel.GameDetails,
            HostName = eventModel.HostName,
            HostUserId = eventModel.HostUserId,
            IsFinalized = eventModel.IsFinalized,
            FoodSuggestions = eventModel.FoodSuggestions
                .OrderByDescending( f => f.UpvoteCount)
                .ThenBy(f => f.FoodSuggestionId)
                .Select(f => new FoodSuggestionDto
            {
                FoodSuggestionId = f.FoodSuggestionId,
                FoodName = f.FoodName,
                SuggestedByName = f.SuggestedByName,
                UpvoteCount = f.UpvoteCount,
                HasUserUpvoted = userVotes.Contains(f.FoodSuggestionId)
            }).ToList()
        };

        return eventDto;
    }

    // POST: api/Events
    [HttpPost]
    public async Task<IActionResult> PostEvent(EventDto eventDto)
    {
        // 1. Convert DTO to Model
        var newEvent = new Event
        {
            EventName = eventDto.EventName,
            EventDate = DateTime.SpecifyKind(eventDto.EventDate, DateTimeKind.Utc),
            GameDetails = eventDto.GameDetails,
            HostUserId = eventDto.HostUserId, 
            HostName = eventDto.HostName,
            IsFinalized = false 
        };

        // 2. Add to DbContext and Save
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        
        //3. Map Id back to DTO for value
        eventDto.EventId = newEvent.EventId;

        // 4. Return a 201 Created status code
        return CreatedAtAction(nameof(GetEvent), new { eventId = newEvent.EventId }, eventDto);
    }
    
    [HttpPost("{eventId}/food")]
    public async Task<IActionResult> PostFood(int eventId, FoodSuggestionDto foodDto)
    {
        var eventModel = await _context.Events.FindAsync(eventId);
        if (eventModel == null) return NotFound();

        var newFood = new FoodSuggestion
        {
            FoodName = foodDto.FoodName,
            SuggestedByName = foodDto.SuggestedByName,
            EventId = eventId
        };

        _context.FoodSuggestions.Add(newFood);
        await _context.SaveChangesAsync();

        return Ok();
    }
    [HttpPost("food/{foodId}/upvote")]
    public async Task<IActionResult> UpvoteFood(int foodId, [FromQuery] string voterName)
    {
        if (string.IsNullOrEmpty(voterName)) return BadRequest("Voter name is required.");
        
            // 1. Check if the vote already exists
        var existingVote = await _context.UserVotes
            .FirstOrDefaultAsync(v => v.FoodSuggestionId == foodId && v.VoterName == voterName);
        
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound(); 
        
        if (existingVote != null)
        {
            // 2. If they already voted, REMOVE it (Toggle off)
            _context.UserVotes.Remove(existingVote);
            food.UpvoteCount = Math.Max(0, food.UpvoteCount - 1);
        }
        else
        {
            // 3. If they haven't voted, ADD it (Toggle on)
            _context.UserVotes.Add(new UserVote 
            { 
                FoodSuggestionId = foodId, 
                VoterName = voterName 
            });
            food.UpvoteCount++;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}
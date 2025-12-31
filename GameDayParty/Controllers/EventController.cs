// GameDayParty/Controllers/EventsController.cs

using GameDayParty.Data;
using GameDayParty.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
    public async Task<ActionResult<EventDto>> GetEvent(int eventId)
    {
        var currentUserName = User.Identity?.Name;
        
        var eventModel = await _context.Events
            .Include(e => e.FoodSuggestions) 
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventModel == null) return NotFound();
        
        var userVotes = new List<int>();
        if (!string.IsNullOrEmpty(currentUserName))
        {
            userVotes = await _context.UserVotes
                .Where(v => v.VoterName == currentUserName)
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
                HasUserUpvoted = userVotes.Contains(f.FoodSuggestionId),
                ClaimedByName = f.ClaimedByName
            }).ToList()
        };

        return eventDto;
    }

    // POST: api/Events
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostEvent(EventDto eventDto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var currentUserName = User.Identity?.Name; 
        // 1. Convert DTO to Model
        var newEvent = new Event
        {
            EventName = eventDto.EventName,
            EventDate = DateTime.SpecifyKind(eventDto.EventDate, DateTimeKind.Utc),
            GameDetails = eventDto.GameDetails,
            HostUserId = userId,
            HostName = currentUserName ?? eventDto.HostName,
            IsFinalized = false 
        };

        // 2. Add to DbContext and Save
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        
        // 3. Return a 201 Created status code for new event
        return CreatedAtAction(nameof(GetEvent), new { eventId = newEvent.EventId }, newEvent);
    }
    
    [Authorize]
    [HttpPost("{eventId}/food")]
    public async Task<IActionResult> PostFood(int eventId, FoodSuggestionDto foodDto)
    {
        var eventModel = await _context.Events.FindAsync(eventId);
        if (eventModel == null) return NotFound();
        
        var currentUserName = User.Identity?.Name;

        var newFood = new FoodSuggestion
        {
            FoodName = foodDto.FoodName,
            SuggestedByName = currentUserName ?? "Guest",
            EventId = eventId
        };

        _context.FoodSuggestions.Add(newFood);
        await _context.SaveChangesAsync();

        return Ok(newFood);
    }
    
    [Authorize]
    [HttpPost("food/{foodId}/upvote")]
    public async Task<IActionResult> UpvoteFood(int foodId)
    {
        var currentUserName = User.Identity?.Name;
        if (string.IsNullOrEmpty(currentUserName)) return Unauthorized();
        
        var existingVote = await _context.UserVotes
            .FirstOrDefaultAsync(v => v.FoodSuggestionId == foodId && v.VoterName == currentUserName);
        
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
                VoterName = currentUserName 
            });
            food.UpvoteCount++;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    
    [HttpPut("food/{foodId}/claim")]
    public async Task<IActionResult> ClaimFood(int foodId)
    {
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        var currentUserName = User.Identity?.Name;
        food.ClaimedByName = currentUserName;
        
        await _context.SaveChangesAsync();
        return Ok(food);
    }
    
    [Authorize]
    [HttpPut("food/{foodId}/unclaim")]
    public async Task<IActionResult> UnclaimFood(int foodId)
    {
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        food.ClaimedByName = null; 
        await _context.SaveChangesAsync();
        return Ok(food);
    }
}
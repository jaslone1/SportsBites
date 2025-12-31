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
    
    // PUT
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventUpdateDto model)
    {
        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null) return NotFound();

        // SECURITY: Ensure the person editing is the owner
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (existingEvent.HostUserId != currentUserId) return Forbid();

        existingEvent.EventName = model.EventName;
        existingEvent.GameDetails = model.GameDetails;
        existingEvent.EventDate = DateTime.SpecifyKind(model.EventDate, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return Ok(existingEvent);
    }
    
    // DELETE
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel == null) return NotFound();

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (eventModel.HostUserId != currentUserId) return Forbid();

        _context.Events.Remove(eventModel);
        await _context.SaveChangesAsync();
        return NoContent(); // 204 success
    }
    
    // PATCH
    [HttpPatch("{id}/finalize")]
    [Authorize]
    public async Task<IActionResult> FinalizeEvent(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel == null) return NotFound();

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (eventModel.HostUserId != currentUserId) return Forbid();

        eventModel.IsFinalized = !eventModel.IsFinalized; // Toggle status
        await _context.SaveChangesAsync();
        return Ok(new { isFinalized = eventModel.IsFinalized });
    }
}
    
    
    
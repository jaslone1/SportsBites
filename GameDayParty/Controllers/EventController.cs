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
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        // 1. Get the current user ID (will be null if they aren't logged in)
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // 2. Filter: Show it if it's public OR if the user is the host
        var events = await _context.Events
            .Where(e => e.IsPublic || e.HostUserId == currentUserId) 
            .Select(e => new EventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDate = e.EventDate,
                GameDetails = e.GameDetails,
                HostName = e.HostName,
                HostUserId = e.HostUserId,
                IsFinalized = e.IsFinalized,
                IsPublic = e.IsPublic 
            })
            .ToListAsync();

        return events;
    }

    [HttpGet("{eventId}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> GetEvent(int eventId)
    {
        var currentUserName = User.Identity?.Name;
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var eventModel = await _context.Events
            .Include(e => e.FoodSuggestions)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventModel == null) return NotFound();

        // PRIVACY CHECK:
        // If it's private and you aren't the host, return Forbid or NotFound
        if (!eventModel.IsPublic && eventModel.HostUserId != currentUserId)
        {
            return Forbid(); 
        }

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
            IsPublic = eventModel.IsPublic, 
            FoodSuggestions = eventModel.FoodSuggestions
                .OrderByDescending(f => f.UpvoteCount)
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

    // POST
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
    
    
    
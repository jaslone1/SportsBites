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

[Authorize]
[Route("api/events")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventController(AppDbContext context) => _context = context;

    // HELPER: Safely gets ID from Token as an Int
    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        var currentUserId = GetUserId();

        // Filter: Show public events OR events hosted by the current user
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

        return Ok(events); // Ok() wraps the list to solve nullability warnings
    }

    [HttpGet("{eventId}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> GetEvent(int eventId)
    {
        var currentUserId = GetUserId();

        var eventModel = await _context.Events
            .Include(e => e.FoodSuggestions)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventModel == null) return NotFound();

        // PRIVACY CHECK
        if (!eventModel.IsPublic && eventModel.HostUserId != currentUserId)
        {
            return Forbid(); 
        }

        var userVotes = new List<int>();
        if (!string.IsNullOrEmpty(currentUserId))
        {
            userVotes = await _context.UserVotes
                .Where(v => v.VoterId == currentUserId) // VoterId must be string in DB
                .Select(v => v.FoodSuggestionId)
                .ToListAsync();
        }

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
                    SuggestedByUserId = f.SuggestedByUserId, 
                    UpvoteCount = f.UpvoteCount,
                    HasUserUpvoted = userVotes.Contains(f.FoodSuggestionId),
                    ClaimedByName = f.ClaimedByName,
                    ClaimedByUserId = f.ClaimedByUserId 
                }).ToList()
        };

        return Ok(eventDto);
    }

    [HttpPost]
    public async Task<IActionResult> PostEvent(EventDto eventDto)
    {
        var currentUserId = GetUserId();
        
        var newEvent = new Event
        {
            EventName = eventDto.EventName,
            EventDate = DateTime.SpecifyKind(eventDto.EventDate, DateTimeKind.Utc),
            GameDetails = eventDto.GameDetails,
            HostUserId = currentUserId, 
            HostName = User.Identity?.Name ?? "Host",
            IsPublic = eventDto.IsPublic,
            IsFinalized = false 
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetEvent), new { eventId = newEvent.EventId }, newEvent);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventUpdateDto model)
    {
        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null) return NotFound();

        // Security check using int
        if (existingEvent.HostUserId != GetUserId()) return Forbid();

        existingEvent.EventName = model.EventName;
        existingEvent.GameDetails = model.GameDetails;
        existingEvent.EventDate = DateTime.SpecifyKind(model.EventDate, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return Ok(existingEvent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel == null) return NotFound();

        if (eventModel.HostUserId != GetUserId()) return Forbid();

        _context.Events.Remove(eventModel);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPatch("{id}/finalize")]
    public async Task<IActionResult> FinalizeEvent(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel == null) return NotFound();

        if (eventModel.HostUserId != GetUserId()) return Forbid();

        eventModel.IsFinalized = !eventModel.IsFinalized;
        await _context.SaveChangesAsync();
        return Ok(new { isFinalized = eventModel.IsFinalized });
    }
}
    
    
    
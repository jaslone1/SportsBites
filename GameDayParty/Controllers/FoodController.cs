// GameDayParty/Controllers/FoodController.cs

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
[Route("api/[controller]")]
[ApiController]
public class FoodController : ControllerBase
{
    private readonly AppDbContext _context;

    public FoodController(AppDbContext context) => _context = context;
    
    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    [HttpPost("{eventId}")]
    public async Task<IActionResult> PostFood(int eventId, FoodSuggestionDto foodDto) 
    { 
        var eventModel = await _context.Events.FindAsync(eventId);
        if (eventModel == null) return NotFound();
        
        var currentUserId = GetUserId();
        var currentUserName = User.Identity?.Name;

        var newFood = new FoodSuggestion
        {
            FoodName = foodDto.FoodName,
            SuggestedByUserId = currentUserId, 
            SuggestedByName = currentUserName ?? "Guest",
            EventId = eventId
        };

        _context.FoodSuggestions.Add(newFood);
        await _context.SaveChangesAsync();

        return Ok(newFood);
    }

    [HttpPost("{foodId}/upvote")]
    public async Task<IActionResult> UpvoteFood(int foodId) 
    { 
        var currentUserId = GetUserId();
        if (currentUserId == 0) return Unauthorized();
        
        // Ensure VoterId is the property name in your UserVote model
        var existingVote = await _context.UserVotes
            .FirstOrDefaultAsync(v => v.FoodSuggestionId == foodId && v.VoterId == currentUserId);
        
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound(); 
        
        if (existingVote != null)
        {
            _context.UserVotes.Remove(existingVote);
            food.UpvoteCount = Math.Max(0, food.UpvoteCount - 1);
        }
        else
        {
            _context.UserVotes.Add(new UserVote 
            { 
                FoodSuggestionId = foodId, 
                VoterId = currentUserId 
            });
            food.UpvoteCount++;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{foodId}/claim")]
    public async Task<IActionResult> ClaimFood(int foodId) 
    { 
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        food.ClaimedByUserId = GetUserId(); 
        food.ClaimedByName = User.Identity?.Name;
        
        await _context.SaveChangesAsync();
        return Ok(food);
    }
    
    [HttpPut("{foodId}/unclaim")]
    public async Task<IActionResult> UnclaimFood(int foodId)
    {
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        // Secure check: Must match the ID of the person who claimed it
        if (food.ClaimedByUserId != GetUserId()) 
        {
            return BadRequest("You can't unclaim someone else's snack!");
        }

        food.ClaimedByUserId = null; // Requires public int? ClaimedByUserId in model
        food.ClaimedByName = null; 
        await _context.SaveChangesAsync();
        return Ok(food);
    }
    
    [HttpDelete("{foodId}")]
    public async Task<IActionResult> DeleteFood(int foodId)
    {
        var food = await _context.FoodSuggestions
            .Include(f => f.Event)
            .FirstOrDefaultAsync(f => f.FoodSuggestionId == foodId);
            
        if (food == null) return NotFound();

        var currentUserId = GetUserId();
    
        bool isSuggester = food.SuggestedByUserId == currentUserId;
        bool isHost = (food.Event?.HostUserId ?? 0) == currentUserId;
        
        //Suggester OR Event Host can delete
        if (!isSuggester && !isHost) 
        {
            return Forbid();
        }

        _context.FoodSuggestions.Remove(food);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/name")]
    public async Task<IActionResult> UpdateFoodName(int id, [FromBody] string newName)
    {
        var food = await _context.FoodSuggestions.FindAsync(id);
        if (food == null) return NotFound();

        if (food.SuggestedByUserId != GetUserId()) return Forbid();

        food.FoodName = newName;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
}
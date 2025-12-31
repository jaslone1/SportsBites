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

    [HttpPost("{eventId}")]
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

    [HttpPost("{foodId}/upvote")]
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
            _context.UserVotes.Remove(existingVote);
            food.UpvoteCount = Math.Max(0, food.UpvoteCount - 1);
        }
        else
        {
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

    [HttpPut("{foodId}/claim")]
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
    [HttpPut("{foodId}/unclaim")]
    public async Task<IActionResult> UnclaimFood(int foodId)
    {
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        var currentUserName = User.Identity?.Name;

        if (food.ClaimedByName != currentUserName) 
        {
            return BadRequest("You can't unclaim someone else's snack!");
        }

        food.ClaimedByName = null; 
        await _context.SaveChangesAsync();
        return Ok(food);
    }
    
    //  DELETE
    [HttpDelete("{foodId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFood(int foodId)
    {
        var food = await _context.FoodSuggestions.FindAsync(foodId);
        if (food == null) return NotFound();

        var currentUserName = User.Identity?.Name;
    
        // Check if user is the one who suggested it
        if (food.SuggestedByName != currentUserName) return Forbid();

        _context.FoodSuggestions.Remove(food);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Stories;
using Tindro.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/stories")]
public class StoryController : ControllerBase
{
    private readonly CommandDbContext _db;

    public StoryController(CommandDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult AddStory([FromBody] string mediaUrl)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var story = new Story
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MediaUrl = mediaUrl,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _db.Stories.Add(story);
        _db.SaveChanges();

        return Ok();
    }

   
}

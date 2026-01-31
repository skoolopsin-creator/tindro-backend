using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Stories;
using Tindro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/stories")]
public class StoryQueryController : ControllerBase
{
    private readonly QueryDbContext _db;

    public StoryQueryController(QueryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetStories()
    {
        var now = DateTime.UtcNow;

        var stories = _db.Stories
            .AsNoTracking()
            .Where(x => x.ExpiresAt > now)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Ok(stories);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tindro.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tindro.Domain.Common;
using Tindro.Api.Extensions;

[ApiController]
[Authorize]
[Route("api/interests")]
public class InterestsController : ControllerBase
{
    private readonly QueryDbContext _db;

    public InterestsController(CommandDbContext db)
    {
        _db = db;
    }

    // 🔹 1. Get Master Interest List
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        var interests = await _db.Interests
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Category,
                x.IconKey
            })
            .ToListAsync();

        return Ok(interests);
    }


 
    [HttpGet("myinterest")]
    public async Task<IActionResult> GetMyInterests()
    {
        var userId = User.GetUserId();

        var interests = await _db.UserInterests
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.Interest.Id,
                x.Interest.Name,
                x.Interest.IconKey
            })
            .ToListAsync();

        return Ok(interests);
    }
}

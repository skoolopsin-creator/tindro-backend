using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Infrastructure.Persistence;
using Tindro.Domain.Match;

[Authorize]
[ApiController]
[Route("api/match")]
public class MatchController : ControllerBase
{
    private readonly CommandDbContext _db;

    public MatchController(CommandDbContext db)
    {
        _db = db;
    }

    [HttpPost("swipe")]
    public IActionResult Swipe([FromBody] SwipeRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (_db.Swipes.Any(x => x.FromUserId == userId && x.ToUserId == req.TargetUserId))
            return BadRequest("Already swiped");

        var swipe = new Swipe
        {
            Id = Guid.NewGuid(),
            FromUserId = userId,
            ToUserId = req.TargetUserId,
            IsLike = req.IsLike
        };

        _db.Swipes.Add(swipe);
        _db.SaveChanges();

        if (req.IsLike)
        {
            var reverseLike = _db.Swipes
                .FirstOrDefault(x => x.FromUserId == req.TargetUserId &&
                                     x.ToUserId == userId &&
                                     x.IsLike);

            if (reverseLike != null)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    User1Id = userId,
                    User2Id = req.TargetUserId
                };

                _db.Matches.Add(match);
                _db.SaveChanges();

                return Ok(new { matched = true });
            }
        }

        return Ok(new { matched = false });
    }
}

public record SwipeRequest(Guid TargetUserId, bool IsLike);

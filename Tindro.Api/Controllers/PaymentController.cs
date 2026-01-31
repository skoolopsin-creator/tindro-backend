using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Payments;
using Tindro.Domain.Match;
using Tindro.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly CommandDbContext _db;

    public PaymentController(CommandDbContext db)
    {
        _db = db;
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe(string plan)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Plan = plan,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1)
        };

        _db.Subscriptions.Add(sub);
        _db.SaveChanges();

        return Ok();
    }

    [HttpPost("boost")]
    public IActionResult ActivateBoost()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var boost = new Boost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActiveUntil = DateTime.UtcNow.AddMinutes(30)
        };

        _db.Boosts.Add(boost);
        _db.SaveChanges();

        return Ok();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Users;
using Tindro.Infrastructure.Persistence;
using Tindro.Application;
using Microsoft.EntityFrameworkCore;


[Authorize]
[ApiController]
[Route("api/users")]
public class UsersQueryController : ControllerBase
{
    private readonly QueryDbContext _db;

    public UsersQueryController(QueryDbContext db)
    {
        _db = db;
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var user = _db.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.Id,
                x.Phone,
                Profile = x.Profile
            })
            .FirstOrDefault();

        return Ok(user);
    }
}

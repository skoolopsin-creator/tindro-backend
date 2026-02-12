using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Users;
using Tindro.Infrastructure.Persistence;
using Tindro.Application;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;


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
    // Read the "sub" claim coming from JWT
    var sub = User.FindFirst("sub")?.Value;

    if (string.IsNullOrEmpty(sub))
        return Unauthorized("sub claim missing");

    // Convert to Guid
    if (!Guid.TryParse(sub, out var userId))
        return Unauthorized("invalid sub claim");

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

    if (user == null)
        return NotFound("user not found");

    return Ok(user);
}

}

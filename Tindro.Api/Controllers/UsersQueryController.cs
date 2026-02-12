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
       var userId = Guid.Parse(
       var userId = User.FindFirst("sub")?.Value;

if (userId == null)
    return Unauthorized("sub claim missing");

var guid = Guid.Parse(userId);


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

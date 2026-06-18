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
    var userId = User.GetUserId();

    var user = _db.Users
        .AsNoTracking()
        .Include(x => x.Profile)
        .FirstOrDefault(x => x.Id == userId);

    if (user == null)
        return NotFound("user not found");

    return Ok(new
    {
        user.Id,
        user.Phone,

        Profile = user.Profile ?? new
        {
            Id = Guid.Empty,
            UserId = user.Id,
            Name = "",
            DateOfBirth = (DateTime?)null,
            Gender = "",
            Bio = "",
            Photos = new List<object>(),
            Interests = new List<string>(),
            MinAgePreference = 18,
            MaxAgePreference = 35,
            GenderPreference = "",
            Education = "",
            IncomeRange = ""
        }
    });
}

}

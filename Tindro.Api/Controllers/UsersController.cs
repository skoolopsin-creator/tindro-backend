using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Domain.Users;
using Tindro.Infrastructure.Persistence;
using Tindro.Application;
using Tindro.Api.Extensions;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly CommandDbContext _db;

    public UsersController(CommandDbContext db)
    {
        _db = db;
    }

    // Create or update profile
    [HttpPost("profile")]
public IActionResult SaveProfile([FromBody] CreateProfileDto dto)
{
    try
    {
        var userId = User.GetUserId();

        var profile = _db.Profiles.FirstOrDefault(x => x.UserId == userId);

        if (profile == null)
        {
            profile = new Profile
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
            _db.Profiles.Add(profile);
        }

        profile.Name = dto.Name;
        profile.DateOfBirth = DateTime.SpecifyKind(
    dto.DateOfBirth,
    DateTimeKind.Utc
);
        profile.Gender = dto.Gender;
        profile.Bio = dto.Bio;
        profile.MinAgePreference = dto.MinAgePreference;
        profile.MaxAgePreference = dto.MaxAgePreference;
        profile.GenderPreference = dto.GenderPreference;
        profile.Education = dto.Education;
        profile.IncomeRange = dto.IncomeRange;

        _db.SaveChanges();

        return Ok(profile);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = ex.Message,
            inner = ex.InnerException?.Message,
            stack = ex.StackTrace
        });
    }
}

  
}

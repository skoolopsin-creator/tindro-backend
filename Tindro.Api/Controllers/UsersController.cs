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
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    // Create or update profile
    [HttpPost("profile")]
    public IActionResult SaveProfile(CreateProfileDto dto)
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
        profile.DateOfBirth = dto.DateOfBirth;
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

  
}

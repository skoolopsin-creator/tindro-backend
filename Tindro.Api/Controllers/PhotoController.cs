using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Api.Extensions;
using Tindro.Domain.Users;
using Tindro.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/photos")]
public class PhotoController : ControllerBase
{
    private readonly CommandDbContext _db;

    public PhotoController(CommandDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult AddPhoto(string url)
    {
        var userId = User.GetUserId();

        var profile = _db.Profiles.First(x => x.UserId == userId);

        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
            Url = url
        };

        _db.Photos.Add(photo);
        _db.SaveChanges();

        return Ok(photo);
    }
}

using Microsoft.AspNetCore.Mvc;
using Tindro.Infrastructure.Persistence;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Users;
using FirebaseAdmin.Auth;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CommandDbContext _db;
private readonly IJwtTokenService _jwt;

public AuthController(CommandDbContext db, IJwtTokenService jwt)

    {
        _db = db;
        _jwt = jwt;
    }
   [HttpPost("firebase")]
public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseAuthRequest req)
{
    try
    {
        var decodedToken = await FirebaseAdmin.Auth.FirebaseAuth
            .DefaultInstance
            .VerifyIdTokenAsync(req.FirebaseToken);

        var firebaseUid = decodedToken.Uid;

        if (!decodedToken.Claims.ContainsKey("phone_number"))
            return Unauthorized("Phone number missing in Firebase token");

        var phone = decodedToken.Claims["phone_number"]?.ToString();

        if (string.IsNullOrEmpty(phone))
            return Unauthorized("Invalid phone number in token");

        Console.WriteLine($"UID: {firebaseUid}");
        Console.WriteLine($"PHONE: {phone}");

        var user = _db.Users.FirstOrDefault(x =>
            x.FirebaseUid == firebaseUid || x.Phone == phone);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Phone = phone,
                IsVerified = true,
                IsShadowBanned = false,
                CreatedAt = DateTime.UtcNow,
                LastActive = DateTime.UtcNow
               
            };

            _db.Users.Add(user);
            _db.SaveChanges();
        }

        var token = _jwt.GenerateToken(user);

        return Ok(new
        {
            token,
            userId = user.Id
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return StatusCode(500, "Internal Server Error");
    }
}


    [HttpPost("send-otp")]
    public IActionResult SendOtp([FromBody] string phone)
    {
        // TODO: Integrate Firebase or SMS Gateway
        return Ok(new { otp = "123456" }); // Mock
    }

    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] OtpRequest req)
    {
        if (req.Otp != "123456")
            return Unauthorized();

        var user = _db.Users.FirstOrDefault(x => x.Phone == req.Phone);

        if (user == null)
        {
            user = new User { Id = Guid.NewGuid(), Phone = req.Phone };
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        var token = _jwt.GenerateToken(user);
        return Ok(new { token });
    }
}

public record OtpRequest(string Phone, string Otp);


public record FirebaseAuthRequest(string FirebaseToken);

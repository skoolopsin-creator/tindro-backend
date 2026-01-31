using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Common.Interfaces;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _repo;

        public NotificationsController(INotificationRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string tab = "all", CancellationToken ct = default)
        {
            var userId = User.GetUserId();
           

            var allowed = new[] { "all", "likes", "matches" };
            if (!allowed.Contains(tab)) tab = "all";

            var items = await _repo.GetNotificationsAsync(userId, tab, ct);
            return Ok(items);
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkRead([FromBody] Guid[] ids, CancellationToken ct = default)
        {
            var userId = User.GetUserId();
          

            var key = $"notifications:read:{userId}";
            var redis = HttpContext.RequestServices.GetService(typeof(Tindro.Application.Common.Interfaces.IRedisService)) as Tindro.Application.Common.Interfaces.IRedisService;
            if (redis != null)
            {
                foreach (var id in ids)
                {
                    await redis.SetAddAsync(key, id.ToString());
                }
            }

            return NoContent();
        }
        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterFcmTokenRequest request, CancellationToken ct = default)
            {
                var userId = User.GetUserId();
               

                if (string.IsNullOrWhiteSpace(request.Token))
                    return BadRequest("Token required");

                await _repo.SaveDeviceTokenAsync(
                    userId,
                    request.Token,
                    request.Platform ?? "unknown",
                    ct
                );

                return NoContent();
            }

    }
}

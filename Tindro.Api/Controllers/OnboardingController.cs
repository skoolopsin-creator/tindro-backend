using Microsoft.AspNetCore.Mvc;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class OnboardingController : ControllerBase
    {
        // GET /api/me
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userId = User.GetUserId();
        

            // Stubbed response; replace with DB lookup if you want persistence
            return Ok(new {
                id = userId,
                onboardingCompleted = false
            });
        }

        // POST /api/users/{userId}/onboarding-complete
        [HttpPost("users/{userId}/onboarding-complete")]
        public IActionResult MarkOnboardingComplete([FromRoute] Guid userId)
        {
            var current = User.GetUserId();
          

            // Basic check: only allow the same user for now
            if (current != userId)
                return Forbid();

            // TODO: persist onboardingCompleted to DB (Profile/User)

            return NoContent();
        }

        // GET /api/onboarding/slides
        [HttpGet("onboarding/slides")]
        public IActionResult GetSlides()
        {
            var slides = new[] {
                new { title = "Connect & Start Talking", text = "Find someone who matches your vibe and start chatting instantly.", image = "/onboard1.png" },
                new { title = "Build Real Bonds", text = "Turn chats into meaningful connections. Stay engaged, express yourself.", image = "/onboard2.png" },
                new { title = "Make It Special", text = "Take your connection beyond the screen. Every story deserves a chance.", image = "/onboard3.png" }
            };
            return Ok(slides);
        }
    }
}

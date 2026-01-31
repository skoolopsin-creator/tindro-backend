using Microsoft.AspNetCore.Mvc;

namespace Tindro.Api.Controllers
{
    public record EventDto(string? UserId, string Event, object? Meta);

    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostEvent([FromBody] EventDto ev)
        {
            // Stub: record analytics/event telemetry. Replace with real pipeline (e.g., persist or push to analytics)
            // For now just return 201 Created.
            return Created(string.Empty, null);
        }
    }
}

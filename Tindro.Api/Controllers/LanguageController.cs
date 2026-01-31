using Microsoft.AspNetCore.Mvc;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api/language")]
    public class LanguageController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLanguage()
        {
            var userId = User.GetUserId();
          

            var current = new { language = "en", supported = new[] { "en", "es", "fr" } };
            return Ok(current);
        }

        [HttpPost]
        public IActionResult SetLanguage([FromBody] string lang)
        {
            var userId = User.GetUserId();
          

            // TODO: persist user language
            return NoContent();
        }
    }
}

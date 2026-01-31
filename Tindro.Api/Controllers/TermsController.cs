using Microsoft.AspNetCore.Mvc;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api/terms-of-service")]
    public class TermsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTerms()
        {
            // Stub: return a simple terms body. Replace with DB or file-based content as needed.
            var terms = new {
                version = "1.0",
                content = "Terms of service placeholder. Replace with real content."
            };
            return Ok(terms);
        }
    }
}

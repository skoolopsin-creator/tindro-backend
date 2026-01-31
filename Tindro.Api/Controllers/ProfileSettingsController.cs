using Microsoft.AspNetCore.Mvc;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileSettingsController : ControllerBase
    {
        public record ProfileSettingsDto(string DisplayName, string Bio, bool ShowAge, bool ShowDistance);
        public record ChangePasswordDto(string CurrentPassword, string NewPassword);
        public record NotificationSettingsDto(bool PushEnabled, bool EmailEnabled);
        public record PrivacySettingsDto(bool IsProfilePublic, bool ShowDistance);

        [HttpGet]
        public IActionResult GetSettings()
        {
            var userId = User.GetUserId();
           

            // Return stubbed settings - replace with DB lookup
            var settings = new ProfileSettingsDto("", "", true, true);
            return Ok(settings);
        }

        [HttpPost]
        public IActionResult SaveSettings([FromBody] ProfileSettingsDto dto)
        {
            var userId = User.GetUserId();
           

            // TODO: persist settings
            return NoContent();
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.GetUserId();
            

            // TODO: validate current password and set new password
            return NoContent();
        }

        [HttpGet("Notifications")]
        public IActionResult GetNotificationSettings()
        {
            var userId = User.GetUserId();
          

            var ns = new NotificationSettingsDto(true, true);
            return Ok(ns);
        }

        [HttpPost("Notifications")]
        public IActionResult SaveNotificationSettings([FromBody] NotificationSettingsDto dto)
        {
            var userId = User.GetUserId();
         

            // TODO: persist
            return NoContent();
        }

        [HttpGet("Privacy")]
        public IActionResult GetPrivacy()
        {
            var userId = User.GetUserId();
         

            var p = new PrivacySettingsDto(true, true);
            return Ok(p);
        }

        [HttpPost("Privacy")]
        public IActionResult SavePrivacy([FromBody] PrivacySettingsDto dto)
        {
            var userId = User.GetUserId();
           

            // TODO: persist
            return NoContent();
        }
    }
}

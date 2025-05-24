using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/notification/smoke")]
    public class NotificationController : ControllerBase
    {
        private static List<string> ExpoTokens = new List<string>();

        [HttpPost("register")]
        public IActionResult RegisterToken([FromBody] string token)
        {
            if (!ExpoTokens.Contains(token))
            {
                ExpoTokens.Add(token);
            }
            return Ok(new { message = "Token registered" });
        }

        public static List<string> GetAllTokens() => ExpoTokens;
    }
}

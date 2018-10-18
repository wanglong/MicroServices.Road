using Microsoft.AspNetCore.Mvc;

namespace LZZ.DEV.Watch.Controllers
{
    [Produces("application/json")]
    public class HealthController : Controller
    {
        [HttpGet]
        [Route("api/Health")]
        public IActionResult Get() => Ok("ok");
    }
}
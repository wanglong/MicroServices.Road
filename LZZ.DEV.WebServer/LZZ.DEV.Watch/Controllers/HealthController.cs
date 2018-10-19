using Microsoft.AspNetCore.Mvc;

namespace LZZ.DEV.Watch.Controllers
{
    [Produces("application/json")]
    public class HealthController : Controller
    {
        /// <summary>
        /// 健康验证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Health")]
        public IActionResult Get() => Ok("ok");
    }
}
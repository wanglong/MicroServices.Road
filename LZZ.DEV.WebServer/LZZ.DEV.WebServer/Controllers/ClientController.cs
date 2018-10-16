using Microsoft.AspNetCore.Mvc;

namespace LZZ.DEV.WebServer.Controllers
{
    [Produces("application/json")]
    public class ClientController : Controller
    {
        [HttpGet]
        [Route("api/Client/{id}")]
        public IActionResult Get(int id) => Ok($"input {id}");
    }
}
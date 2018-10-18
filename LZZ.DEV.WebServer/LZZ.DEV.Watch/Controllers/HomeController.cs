using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace LZZ.DEV.Watch.Controllers
{
    [Produces("application/json")]
    public class HomeController : Controller
    {
        [HttpPost]
        [Route("/notice")]
        public IActionResult Notice()
        {
            var stream = HttpContext.Request.Body;
            if (HttpContext.Request.ContentLength != null)
            {
                var buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                var content = Encoding.UTF8.GetString(buffer);

                var path = $"{AppDomain.CurrentDomain.BaseDirectory}{DateTime.Now:hh_mm_ss_ffff}.log";
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                }

                using (var sw = new StreamWriter(path))
                {
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
                
                return Ok();
            }
            throw new Exception("post is null");
        }
    }
}
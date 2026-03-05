using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        // API Test
        [HttpGet("/")]
        [ApiKey]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
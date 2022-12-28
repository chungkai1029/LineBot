using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LineBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(dynamic request)
        {
            return Ok(request);
        }
    }
}

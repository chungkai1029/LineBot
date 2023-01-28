using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LineBot.Filters;

namespace LineBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        [HttpPost]
        [LineVerifySignature]
        public async Task<IActionResult> Post(dynamic request)
        {
            Console.WriteLine(request);
            return Ok(request);
        }
    }
}

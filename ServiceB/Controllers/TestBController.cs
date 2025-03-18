using Microsoft.AspNetCore.Mvc;

namespace ServiceB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestBController : ControllerBase
    {
        [HttpGet("data")]
        public IActionResult GetData()
        {
            return Ok(new { data = "Sample data from ServiceB - TestBController!" });
        }

        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new { health = "ServiceB is healthy", timestamp = DateTime.UtcNow });
        }
    }
}

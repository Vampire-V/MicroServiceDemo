using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceA.Authorization;

namespace ServiceA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAController : ControllerBase
    {
        [HttpGet("message")]
        [Authorize(Policy = "UserView")]
        public IActionResult GetMessage()
        {
            return Ok(new { message = "Hello from ServiceA - TestAController!" });
        }

        [HttpGet("status")]
        [MultiPolicyAuthorize("UserCreate", "UserEdit")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "ServiceA is running", timestamp = DateTime.UtcNow });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "API is running successfully.",
                timestamp = DateTime.UtcNow,
                version = "1.1.0",
                message = "HMRC-TAX-FLOW Authentication API"
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace TraineeManagemnt.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var response = new
            {
                status = "Healthy",
                application = "TraineeManagement",
                timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }
    }
}
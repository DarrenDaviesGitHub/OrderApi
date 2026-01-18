using Microsoft.AspNetCore.Mvc;

namespace Ardent.OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public async Task<IActionResult> Get() => Ok("Order API is online");
    }
}

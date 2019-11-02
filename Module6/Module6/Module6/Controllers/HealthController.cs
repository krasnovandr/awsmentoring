using Microsoft.AspNetCore.Mvc;

namespace Module6.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "ok";
        }
    }
}

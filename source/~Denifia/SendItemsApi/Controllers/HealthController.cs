using Microsoft.AspNetCore.Mvc;

namespace Denifia.Stardew.SendItemsApi.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        // GET api/health
        [HttpGet()]
        public bool Get()
        {
            return true;
        }
    }
}

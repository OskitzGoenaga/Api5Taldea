using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErreserbakController : ControllerBase
    {
        // POST api/<ErreserbakController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}

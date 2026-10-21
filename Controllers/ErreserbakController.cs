using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErreserbakController : ControllerBase
    {
        // GET: api/<ErreserbakController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ErreserbakController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ErreserbakController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ErreserbakController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ErreserbakController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

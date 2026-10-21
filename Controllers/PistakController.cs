using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PistakController : ControllerBase
    {
        // GET: api/<PistakController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PistakController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PistakController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PistakController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PistakController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

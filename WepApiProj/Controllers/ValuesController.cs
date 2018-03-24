using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;

namespace WepApiProj.Controllers
{
    
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        

        static TopicClient queueClient = new TopicClient(Constants.SbConnectionString, "demo");
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(id.ToString())));
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IventoryController : ControllerBase
    {
        [HttpPost]
        public int Update([FromBody] Inventory inventory)
        {
            throw new Exception("Error create order");
            Console.WriteLine($"Update inventory name:{inventory.Name}");
            return 3;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Console.WriteLine($"Delete inventory id:{id}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        [HttpPost]
        public int NotifySuccess([FromBody] Notify notify)
        {
            Console.WriteLine($"Send nofication for product: {notify.Name}");
            return 2;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Console.WriteLine($"Send rollback transaction with  id:{id}");
        }
    }
}
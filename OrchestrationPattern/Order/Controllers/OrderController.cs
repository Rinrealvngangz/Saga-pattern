using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpPost]
      public  int CreateOrder([FromBody] Order order)
      {
          Console.WriteLine($"Create order with name: {order.Name} - quantity {order.Quantity}");
          return 1;
      }

      [HttpDelete("{id}")]
      public void Delete(int id)
      {
          Console.WriteLine($"Delete order with id:{id}");
      }
    }
}
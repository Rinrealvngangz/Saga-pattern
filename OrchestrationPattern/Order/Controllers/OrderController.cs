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
      public  OrderResponse CreateOrder([FromBody] Order order)
      {
          Console.WriteLine($"Create order with name: {order.Name} - quantity {order.Quantity}");
          return new OrderResponse
          {
              id = 1,
              Success = true,
              Reason = ""
          };
      }

      [HttpDelete("{id}")]
      public void Delete(int id)
      {
          Console.WriteLine($"Delete order with id:{id}");
      }
    }
}
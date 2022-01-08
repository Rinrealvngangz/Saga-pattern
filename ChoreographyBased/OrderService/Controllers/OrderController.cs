using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RabbitMQ.Client;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ICreateOrder _createOrder;
        private readonly IPublisher<OrderDetail,List<int>,int> _publisher;
        private readonly IModel? channel;
        public OrderController(ICreateOrder createOrder,
                                IPublisher<OrderDetail,List<int>,int> publisher,
                                IConnectionProvider connectionProvider )
        {
            _createOrder = createOrder;
            _publisher = publisher;
            channel = connectionProvider.GetConnection().CreateModel();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreate orderCreate )
        {
            var id =  await _createOrder.Create(orderCreate);
            List<int> data = new List<int> {id};
            _publisher.SetData(data);
            _publisher.Publish(channel,
                      "order-create-exchange",
                               "order.checkout",
                        null);
            return Ok();
        }
    }
}
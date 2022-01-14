using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OrchestrationPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<OrderResponse> Post([FromBody] Order order)
        {
           var orderManager = new OrderManager();
          var isSuccess = orderManager.CreatOrder(order, _httpClientFactory);
          return isSuccess
              ? new OrderResponse
              {
                  id = 100,
                  Success = true,
                  Reason = "Thanh cong"
              }
              : new OrderResponse
              {
                  id = 0,
                  Success = false,
                  Reason = "That bai"
              };

        }
    }
}
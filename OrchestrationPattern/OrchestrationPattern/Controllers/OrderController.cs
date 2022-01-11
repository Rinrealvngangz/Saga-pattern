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
            var request = JsonConvert.SerializeObject(order);
           var orderClient = _httpClientFactory.CreateClient("Order");
          var orderResponse = await orderClient.PostAsync("/api/order",
                                new StringContent(request,Encoding.UTF8,"application/JSON"));
          var orderId = await  orderResponse.Content.ReadAsStringAsync();
          string inventoryId = string.Empty;
          try
          {
              var inventoryClient =  _httpClientFactory.CreateClient("Inventory");
              var inventoryResponse = await inventoryClient.PostAsync("/api/iventory",
                  new StringContent(request, Encoding.UTF8, "application/JSON"));
              if (inventoryResponse.StatusCode != HttpStatusCode.OK)
              {
                  throw new Exception(inventoryResponse.ReasonPhrase);
              }
              inventoryId = await inventoryResponse.Content.ReadAsStringAsync();
          }
          catch (Exception e)
          {
              await orderClient.DeleteAsync($"/api/order/{orderId}");
              return new OrderResponse
              {
                  id = orderId,
                  Success = "false",
                  Reason = e.Message
              };

          }
      
         var notifyClient =  _httpClientFactory.CreateClient("Notify");
        var notifyResponse = await notifyClient.PostAsync("/api/notify",
             new StringContent(request, Encoding.UTF8, "application/JSON"));
        var notifyId = await notifyResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Orderid:{orderId} - inventoryId :{inventoryId} - notifyId :{notifyId}");
        return new OrderResponse{id = orderId,Success = "success"};
        }
    }
}
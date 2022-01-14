using System.Text;
using Newtonsoft.Json;
using Stateless;
namespace OrchestrationPattern;

public class OrderManager
{
    enum OrderTransactionsState{
      NotStarted,
      OrderCreated,
      OrderCancelled,
      InventoryUpdated,
      InventoryUpdatedFailed,
      InventoryRolledBack,
      NotificationSend,
      NotificationSendFailed
    }

    enum OrderAction
    {
        CreateOrder,
        Cancelled,
        UpdatedInventory,
        RolledBackInventory,
        SendNotification,
        RetryNotification
    }

    public bool CreatOrder(Order order,IHttpClientFactory _httpClientFactory)
    {
        var inventoryId = -1;
        var orderId = -1;
        var request = JsonConvert.SerializeObject(order);
        
        var orderStateMachine = new StateMachine<OrderTransactionsState, OrderAction>(
            OrderTransactionsState.NotStarted);
        orderStateMachine.Configure(OrderTransactionsState.NotStarted)
            .PermitDynamic(OrderAction.CreateOrder, ()  =>
            {

                var orderResult = Order(request, _httpClientFactory); 
                orderId = orderResult.Result.id;
                return orderResult.Result.Success
                    ? OrderTransactionsState.OrderCreated
                    : OrderTransactionsState.OrderCancelled;
            });
        
        orderStateMachine.Configure(OrderTransactionsState.OrderCreated)
            .PermitDynamic(OrderAction.UpdatedInventory,  ()  =>
            {
             var inventoryResult =   InventoryUpdate(request, _httpClientFactory);
                inventoryId = inventoryResult.Result.id;
                return inventoryResult.Result.Success
                    ? OrderTransactionsState.InventoryUpdated
                    : OrderTransactionsState.InventoryUpdatedFailed;
            })
            .OnEntry(()=>orderStateMachine.Fire(OrderAction.UpdatedInventory));
        
        orderStateMachine.Configure(OrderTransactionsState.InventoryUpdated)
            .PermitDynamic(OrderAction.SendNotification,  () =>
            {

                var notifyResult = SendNotify(request, _httpClientFactory);
                return notifyResult.Result.Success
                    ? OrderTransactionsState.NotificationSend
                    : OrderTransactionsState.NotificationSendFailed;
            })
            .OnEntry(()=>orderStateMachine.Fire(OrderAction.SendNotification));
             
        orderStateMachine.Configure(OrderTransactionsState.InventoryUpdatedFailed)
            .PermitDynamic(OrderAction.RolledBackInventory, () =>
            {
                RollBackInventory(inventoryId, _httpClientFactory); 
               return OrderTransactionsState.InventoryRolledBack;

            })
            .OnEntry(()=>orderStateMachine.Fire(OrderAction.RolledBackInventory));
        
        orderStateMachine.Configure(OrderTransactionsState.InventoryRolledBack)
            .PermitDynamic(OrderAction.Cancelled,  () =>
            {
                CancelOrder(orderId,_httpClientFactory); 
                return OrderTransactionsState.OrderCancelled;

            })
            .OnEntry(()=>orderStateMachine.Fire(OrderAction.Cancelled));
        
        orderStateMachine.Fire(OrderAction.CreateOrder);
        
        return orderStateMachine.State == OrderTransactionsState.NotificationSend;
    }

    private async Task<OrderResponse> InventoryUpdate(string request ,IHttpClientFactory _httpClientFactory)
    {
        var inventoryClient =  _httpClientFactory.CreateClient("Inventory");
        var inventoryResponse = await inventoryClient.PostAsync("/api/iventory",
            new StringContent(request, Encoding.UTF8, "application/JSON"));
               
        var inventoryResult = await inventoryResponse.Content.ReadFromJsonAsync<OrderResponse>();
        return inventoryResult;
    }
    
    private async Task<OrderResponse> Order(string request ,IHttpClientFactory _httpClientFactory)
    {
        var orderClient = _httpClientFactory.CreateClient("Order");
        var orderResponse = await orderClient.PostAsync("/api/order",
            new StringContent(request, Encoding.UTF8, "application/JSON"));
        var orderResult = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>();
        return orderResult;
    }
    
    private async Task<OrderResponse> SendNotify(string request ,IHttpClientFactory _httpClientFactory)
    {
        var notifyClient =  _httpClientFactory.CreateClient("Notify");
        var notifyResponse = await notifyClient.PostAsync("/api/notify",
            new StringContent(request, Encoding.UTF8, "application/JSON"));
        var notifyResult = await notifyResponse.Content.ReadFromJsonAsync<OrderResponse>();
        return notifyResult;
    }
    
    private async Task RollBackInventory(int inventoryId,IHttpClientFactory _httpClientFactory)
    {
        var inventoryClient =  _httpClientFactory.CreateClient("Inventory");
        await inventoryClient.DeleteAsync($"/api/iventory/{inventoryId}");
    }
    
    private async Task CancelOrder(int orderId,IHttpClientFactory _httpClientFactory)
    {
        var inventoryClient =  _httpClientFactory.CreateClient("Order");
        await inventoryClient.DeleteAsync($"/api/order/{orderId}");
    }
}
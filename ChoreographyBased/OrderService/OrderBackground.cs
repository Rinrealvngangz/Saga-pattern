using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.Extensions.Hosting;
using Models;
using RabbitMQ.Client;

namespace OrderService
{
    public class OrderBackground : IHostedService
    {
        private readonly ISubscribe<List<CustomerResponse>> _subscribe;
        private readonly IModel _channel;
        private readonly IConnectionProvider _connectionProvider;
        private readonly IUpdateProduct _updateProduct;
        private readonly IDeleteOrder _deleteOrder;
        public OrderBackground(ISubscribe<List<CustomerResponse>> subscribe,
                                IConnectionProvider connectionProvider,
                                IDeleteOrder deleteOrder,
                                IUpdateProduct updateProduct)
        {
            _subscribe = subscribe;
            _connectionProvider = connectionProvider;
           _channel = _connectionProvider.GetConnection().CreateModel();
           _deleteOrder = deleteOrder;
           _updateProduct = updateProduct;
        }
        public  Task StartAsync(CancellationToken cancellationToken)
        {
             _subscribe.Subscribe(_channel,"orderResponse-exchange",
                            "orderResponse-queue","order.*",callback);
             return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
         return Task.CompletedTask;
        }

        private async void callback(List<CustomerResponse> customerResponses)
        {
            var customerResponse = customerResponses[0];
            if (customerResponse.IsSuccess)
            {
               await _updateProduct.UpdateQuantity(customerResponse.IdOrder);
            }
            else
            {
              await  _deleteOrder.Delete(customerResponse.IdOrder);
            }
        }
    }
}
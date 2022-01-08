using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.Extensions.Hosting;
using Models;
using RabbitMQ.Client;

namespace CustomerService
{
    public class CustomerBackground : IHostedService
    {
        private readonly ISubscribe<List<int>> _subscribe;
        private readonly IModel _channel;
        private readonly IConnectionProvider _connectionProvider;
        private readonly ICustomerCreditCard _customerCreditCard;
        private readonly IPublisher<CustomerResponse, List<CustomerResponse>, CustomerResponse> _publisher;

        public CustomerBackground(ISubscribe<List<int>> subscribe,
            IPublisher<CustomerResponse,List<CustomerResponse>,CustomerResponse> publisher,
            IConnectionProvider connectionProvider,
            ICustomerCreditCard customerCreditCard
            )
        {
            _subscribe = subscribe;
            _publisher = publisher;
            _customerCreditCard = customerCreditCard;
            _connectionProvider = connectionProvider;
            _channel = _connectionProvider.GetConnection().CreateModel();

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
           
            _subscribe.Subscribe(_channel,"order-create-exchange",
                "payment-queue","order.*",callback);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           return Task.CompletedTask;
        }
        private async void callback(List<int> idOrder)
        {
          bool isPayment =  await _customerCreditCard.CreditCard(idOrder[0]);
          var customer = new CustomerResponse
          {
              IsSuccess = true,
              IdOrder = idOrder[0]
          };
          
          if (isPayment)
          {
              var dataResponse = new List<CustomerResponse>{customer};
              _publisher.SetData(dataResponse);
              _publisher.Publish(_channel, "orderResponse-exchange", "order.payment", null);
          }
          else
          {
              customer.IsSuccess = false;
              var dataResponse = new List<CustomerResponse>{customer};
              _publisher.SetData(dataResponse);
              _publisher.Publish(_channel, "orderResponse-exchange", "order.payment", null);
          }
                
        }
    }
}
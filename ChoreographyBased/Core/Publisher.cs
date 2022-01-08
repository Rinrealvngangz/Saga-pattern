using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Core
{
    public class Publisher<T,U,X> : IPublisher<T,U,X> where T :class where U: IEnumerable<X>
    {
        private  U _order;
        private bool _disposed;
        private IModel _channel;
        public void Publish(IModel channel, string exchange,string routingKey, IDictionary<string, object>? argument)
        {
            _channel = channel;
            _channel.ExchangeDeclare(exchange,ExchangeType.Topic,arguments: argument);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_order));
            _channel.BasicPublish(exchange,routingKey,null,body);
        }

        public void SetData(U order)
        {
            _order = order;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _channel?.Close();

            _disposed = true;
        }
    }
}
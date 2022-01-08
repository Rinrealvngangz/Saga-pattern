using System;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace Core
{
    public interface IPublisher<T,U,X> : IDisposable where T : class where U : IEnumerable<X> 
    {
        void Publish(IModel channel, string exchange,string routingKey, IDictionary<string, object>? argument);
        void SetData(U list);
    }
}
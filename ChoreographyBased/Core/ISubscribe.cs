using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public interface ISubscribe<T>  where  T : class
    {
        void Subscribe(IModel channel, string exchange, string queue, string routingKey, Action<T> callback);
        IEnumerable<T> GetData();
    }
}
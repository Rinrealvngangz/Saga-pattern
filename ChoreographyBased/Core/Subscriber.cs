using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core
{
    public class Subscriber<T> : ISubscribe<T> where T: class
    {
        private IEnumerable<T> _list;

        public Subscriber()
        {
            _list = new List<T>();
        }
        public void Subscribe(IModel channel, string exchange, string queue, string routingKey,Action<T> callback)
        {
            channel.ExchangeDeclare(exchange,ExchangeType.Topic);
            channel.QueueDeclare(queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments:null);
            channel.QueueBind(queue,exchange,routingKey);
            channel.BasicQos(0,10,false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var data = Encoding.UTF8.GetString(body);
                Console.WriteLine(data);
                T response = JsonConvert.DeserializeObject<T>(data);
                callback.Invoke(response);
            };
            channel.BasicConsume(queue, true, consumer);
        }

        public IEnumerable<T> GetData()
        {
            return _list;
        }
    }
}
using System;
using RabbitMQ.Client;
namespace Core
{
   
        public interface IConnectionProvider : IDisposable
        {
            IConnection GetConnection();
        } 
}
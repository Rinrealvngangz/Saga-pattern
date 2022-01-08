using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Models;
using MySql;
using MySql.Data.MySqlClient;

namespace OrderService
{
    public class CreateOrder : ICreateOrder
    {
        private readonly string connectionString;
        private readonly ILogger<OrderCreate> logger;

        public CreateOrder(string connectionString, ILogger<OrderCreate> logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public async Task<int> Create(OrderCreate orderCreate)
        {
            await using var conn = new MySqlConnection(this.connectionString);
            conn.Open();
          await using var transaction =  await conn.BeginTransactionAsync();
           try
           {
               var id =  conn.QueryFirstOrDefault<int>("Create_Order", new { productId = orderCreate.ProductId, quantity = orderCreate.Quantity, customerId = orderCreate.CustomerId }, 
                              transaction: transaction, commandType:CommandType.StoredProcedure );
              
               transaction.Commit();
               logger.LogInformation($"Create order id:{id} success");
               return id;
           }
           catch(Exception exc)
           {
               logger.LogError($"Error: {exc}");
               transaction.Rollback();
               return -1;
           }
        }

    }
}
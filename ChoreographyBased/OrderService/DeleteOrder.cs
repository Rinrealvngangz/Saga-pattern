using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
namespace OrderService
{
    public class DeleteOrder : IDeleteOrder
    {
        private readonly ILogger<DeleteOrder> _logger;
        private readonly string _connect;
        public DeleteOrder(string connect,ILogger<DeleteOrder> logger)
        {
            _connect = connect;
            _logger = logger;

        }
        public async Task Delete(int id)
        {
            using var connect = new MySqlConnection(_connect);
            connect.Open();
            using var transaction = connect.BeginTransaction();
            try
            {
                await connect.ExecuteAsync("Delete_Order", new { orderId = id }, 
                    transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();
                _logger.LogInformation($"Delete order id:{id} success");
            }
            catch (Exception e)
            {
               transaction.Rollback();  
            } 
        }
    }
}
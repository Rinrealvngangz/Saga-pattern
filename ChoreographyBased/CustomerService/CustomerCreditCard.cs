using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;
using Models;
using MySql;
using MySql.Data.MySqlClient;

namespace CustomerService
{
    public class CustomerCreditCard : ICustomerCreditCard
    {
        private readonly ILogger<CustomerCreditCard> _logger;
        private readonly string _conn;
        private int TotalCard = 5000;
        public CustomerCreditCard(string conn,ILogger<CustomerCreditCard> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        public async Task<bool> CreditCard(int orderIds)
        {
            _logger.LogInformation($"Order id:{orderIds}");
            using var connect = new MySqlConnection(_conn);
            connect.Open();
            using var transaction = connect.BeginTransaction();
            try
            {
                _logger.LogInformation("Started executed...");
                string sql = "Select * From Ecomm.Order Where Id = @orderId;";
                string queryProduct = "Select * From Product Where Id = @productId;";
                
                var orderDetail  = await connect.QuerySingleOrDefaultAsync<OrderDetail>(sql, new {orderId = orderIds}, transaction: transaction);
                Console.WriteLine(orderDetail.Id);
                var productDetail  = await connect.QuerySingleOrDefaultAsync<Product>(queryProduct, new {productId = orderDetail.ProductId}, transaction: transaction);
                int amount = productDetail.Price * orderDetail.Quantity;
                if (amount < TotalCard)
                {
                    TotalCard -= amount;
                    transaction.Commit();
                    _logger.LogInformation($"Payment product id:{orderDetail.ProductId} success");
                    return true;
                }
                _logger.LogInformation($"Payment product id:{orderDetail.ProductId} fail");
                    return false;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error:{e}");
               transaction.Rollback();
               return false;
            } 
        }
    }
}
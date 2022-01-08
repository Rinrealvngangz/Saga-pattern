using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Models;
using MySql.Data.MySqlClient;
using Npgsql;

namespace OrderService
{
    public class UpdateProduct : IUpdateProduct
    {
        private readonly string _connString;
        private readonly ILogger<UpdateProduct> _logger;
        public UpdateProduct(string connString,ILogger<UpdateProduct> logger)
        {
            _connString = connString;
            _logger = logger;
        }
        public async Task UpdateQuantity(int orderIds)
        {
            using var conn = new MySqlConnection(_connString);
             conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            { 
                string sql = "Select * From Ecomm.Order Where Id = @orderId;";
                string queryProduct = "Select * From Product Where Id = @productId;";
                
              var orderDetail  = await conn.QuerySingleOrDefaultAsync<OrderDetail>(sql, new {orderId = orderIds}, transaction: transaction);
              var productDetail  = await conn.QuerySingleOrDefaultAsync<Product>(queryProduct, new {productId = orderDetail.ProductId}, transaction: transaction);
              int sl = productDetail.Quantity - orderDetail.Quantity;
              Console.WriteLine($"quantity:{sl}");
              await conn.ExecuteAsync("Update_Quantity_Product", new {productId = orderDetail.ProductId,quantity = sl}, transaction: transaction,commandType:CommandType.StoredProcedure); 
              transaction.Commit();
              _logger.LogInformation($"Update quantity product id:{orderDetail.ProductId} success");
            }
            catch (Exception e)
            {
             transaction.Rollback();
            }           
        }
    }
}
using System.Threading.Tasks;

namespace OrderService
{
    public interface IUpdateProduct
    {
        Task UpdateQuantity(int orderId);
    }
}
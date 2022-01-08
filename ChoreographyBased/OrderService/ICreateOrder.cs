using System.Threading.Tasks;
using Models;

namespace OrderService
{
    public interface ICreateOrder
    {
        public Task<int> Create(OrderCreate orderCreate);
    }
}
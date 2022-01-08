using System.Threading.Tasks;

namespace OrderService
{
    public interface IDeleteOrder
    {
        Task Delete(int id);
    }
}
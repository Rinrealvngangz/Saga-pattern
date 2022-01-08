using System.Threading.Tasks;

namespace CustomerService
{
    public interface ICustomerCreditCard
    {
        Task<bool> CreditCard(int orderId);
    }
}
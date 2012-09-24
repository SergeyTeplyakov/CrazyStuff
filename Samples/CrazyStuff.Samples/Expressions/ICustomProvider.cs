using System.Threading.Tasks;

namespace CrazyStuff.Samples.Expressions
{
    public interface ICustomProvider
    {
        Task<int> GetNextId();
        Task<string> GetCustomerName(int id);
        Task<Order> GetOrder(int orderId, string customerName);
    }
}
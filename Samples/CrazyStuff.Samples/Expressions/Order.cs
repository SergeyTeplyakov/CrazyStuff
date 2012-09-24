namespace CrazyStuff.Samples.Expressions
{
    public class Order
    {
        public Order(int orderId, string customerName)
        {
            OrderId = orderId;
            CustomerName = customerName;
        }

        public int OrderId { get; set; }
        public string CustomerName { get; set; }
    }
}
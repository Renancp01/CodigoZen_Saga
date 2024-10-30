namespace Orders.Contracts.Order;

public class OrderRejected
{
    public Guid OrderId { get; set; }
    
    public string Reason { get; set; }
}
namespace Orders.Contracts.Order;

public class OrderStatusResult
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
}
namespace Orders.Contracts.Order;

public class OrderSubmitted
{
    public Guid OrderId { get; set; }

    public string CustomerNumber { get; set; }
}
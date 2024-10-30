namespace Orders.Models;

public class KafkaOrderEvent
{
    public Guid OrderId { get; set; }
    public string CustomerNumber { get; set; }
}
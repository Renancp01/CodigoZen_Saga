using Orders.Models;

namespace Orders.Domain;

public class ReceivedOrderEvent : EventBase
{
    public ReceivedOrderEvent(KafkaOrderEvent orderEvent)
    {
        OrderId = orderEvent.OrderId;
        Name = orderEvent.Name;
        CustomerName = orderEvent.CustomerNumber;
    }

    public Guid OrderId { get; set; }

    public string Name { get; set; }

    public string CustomerName { get; set; }

    public override EventType Type => EventType.ReceivedOrder;
}
namespace Orders.Domain;

public abstract class EventBase
{
    public abstract EventType Type  { get; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
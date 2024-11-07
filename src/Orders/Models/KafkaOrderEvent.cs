using System.Text.Json.Serialization;

namespace Orders.Models;

public class KafkaOrderEvent
{
    public Guid OrderId { get; set; }
    
    [JsonPropertyName("Name-1")]
    public string Name { get; set; }
    
    public string CustomerNumber { get; set; }
}
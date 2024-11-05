using MassTransit;
using MongoDB.Bson.Serialization.Attributes;

namespace Orders.Models;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    [BsonId]
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = "Initial";

    public string CustomerNumber { get; set; }
    
    public string Reason { get; set; }

    public int Version { get; set; } // Controle de versão para concorrência otimista

    // Propriedades adicionais conforme necessário
}
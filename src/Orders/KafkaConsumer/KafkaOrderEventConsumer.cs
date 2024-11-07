using MassTransit;
using Orders.Contracts.Order;
using Orders.Models;

namespace Orders.KafkaConsumer;

public class KafkaOrderEventConsumer :IConsumer<KafkaOrderEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public KafkaOrderEventConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<KafkaOrderEvent> context)
    {
        // throw new Exception();
        
        // Publicar o evento OrderSubmitted via RabbitMQ
        await _publishEndpoint.Publish(new OrderSubmitted
        {
            OrderId = context.Message.OrderId,
            CustomerNumber = context.Message.CustomerNumber
            // Outros detalhes do pedido
        });
    }
}
using MassTransit;
using Orders.Contracts.Order;

public class OrderAcceptedConsumer : IConsumer<OrderAccepted>
{
    public async Task Consume(ConsumeContext<OrderAccepted> context)
    {
        var orderId = context.Message.OrderId;
        // Lógica para lidar com o pedido aceito
        // Exemplo: Atualizar o status do pedido no banco de dados, enviar e-mail de confirmação, etc.

        // Simulação de ação
        Console.WriteLine($"Pedido {orderId} foi aceito.");
        await Task.CompletedTask;
    }
}
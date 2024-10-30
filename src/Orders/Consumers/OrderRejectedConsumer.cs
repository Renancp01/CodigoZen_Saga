using MassTransit;
using Orders.Contracts.Order;

namespace Orders.Consumers;

public class OrderRejectedConsumer : IConsumer<OrderRejected>
{
    public async Task Consume(ConsumeContext<OrderRejected> context)
    {
        var orderId = context.Message.OrderId;
        var reason = context.Message.Reason;
        // Lógica para lidar com o pedido rejeitado
        // Exemplo: Atualizar o status do pedido no banco de dados, enviar e-mail com o motivo da rejeição, etc.

        // Simulação de ação
        Console.WriteLine($"Pedido {orderId} foi rejeitado. Motivo: {reason}");
        await Task.CompletedTask;
    }
}
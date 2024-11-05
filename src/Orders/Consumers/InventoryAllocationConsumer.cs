// Consumers/InventoryAllocationConsumer.cs

using MassTransit;
using Orders.Contracts.Inventory;

namespace Orders.Consumers;

public class InventoryAllocationConsumer : IConsumer<InventoryAllocationRequested>
{
    public async Task Consume(ConsumeContext<InventoryAllocationRequested> context)
    {
        // Lógica para verificar disponibilidade no estoque
        bool isAvailable = true; // Simulação

        throw new Exception();
        if (isAvailable)
        {
            await context.Publish(new InventoryAllocationConfirmed
            {
                OrderId = context.Message.OrderId
            });
        }
        else
        {
            await context.Publish(new InventoryAllocationFailed
            {
                OrderId = context.Message.OrderId,
                Reason = "Item out of stock"
            });
        }
    }
}
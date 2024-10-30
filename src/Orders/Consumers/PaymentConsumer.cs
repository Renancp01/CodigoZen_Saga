// Consumers/PaymentConsumer.cs

using MassTransit;
using Orders.Contracts.Payment;

namespace Orders.Consumers;

public class PaymentConsumer : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        bool paymentSuccess = SimulatePaymentProcessing(context.Message.Amount);

        if (paymentSuccess)
        {
            await context.Publish(new PaymentProcessed
            {
                OrderId = context.Message.OrderId,
                Success = true
            });
        }
        else
        {
            await context.Publish(new PaymentProcessed
            {
                OrderId = context.Message.OrderId,
                Success = false,
                FailureReason = "Payment declined"
            });
        }
        
    }
    private bool SimulatePaymentProcessing(decimal amount)
    {
        return amount < 1000;
    }
}
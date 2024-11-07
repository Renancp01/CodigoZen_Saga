// StateMachines/OrderStateMachine.cs

using MassTransit;
using Orders.Contracts.Inventory;
using Orders.Contracts.Order;
using Orders.Contracts.Payment;
using Orders.Machine.States;
using Orders.Models;

namespace Orders.Machine;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        //Todo verificar para inserir StatusInicialSempre
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmittedEvent, x =>
        {
            x.CorrelateById(context => context.Message.OrderId);
            x.InsertOnInitial = true;
         
            x.SetSagaFactory(context => new OrderState
            {
                CorrelationId = context.Message.OrderId,
                CustomerNumber = context.Message.CustomerNumber
            });
        });

        Event(() => InventoryAllocationConfirmedEvent,
            x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryAllocationFailedEvent,
            x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessedEvent, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderSubmittedEvent)
                .Then(context =>
                {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                })
                .TransitionTo(Submitted)
                .SendAsync(new Uri("queue:inventory_service_queue"), context =>
                    context.Init<InventoryAllocationRequested>(new
                    {
                        OrderId = context.Instance.CorrelationId,
                        ItemNumber = "Item123",
                        Quantity = 1
                    }))
        );

        During(Submitted,
            When(InventoryAllocationConfirmedEvent)
                .TransitionTo(PaymentProcessing)
                .SendAsync(new Uri("queue:payment_service_queue"), context => context.Init<ProcessPayment>(new
                {
                    OrderId = context.Instance.CorrelationId,
                    Amount = 100.00m
                })),
            When(InventoryAllocationFailedEvent)
                .Then(context => { context.Instance.Reason = context.Data.Reason; })
                .SendAsync(new Uri("queue:order_rejected_queue"), context => context.Init<OrderRejected>(new
                {
                    OrderId = context.Instance.CorrelationId,
                    Reason = context.Instance.Reason
                }))
                .Finalize()
        );

        During(PaymentProcessing,
            When(PaymentProcessedEvent, context => context.Data.Success)
                .SendAsync(new Uri("queue:order_accepted_queue"), context => context.Init<OrderAccepted>(new
                {
                    OrderId = context.Instance.CorrelationId
                })).TransitionTo(Completed)
                .Finalize(),
            When(PaymentProcessedEvent, context => !context.Data.Success)
                .Then(context => { context.Instance.Reason = context.Data.FailureReason; })
                .SendAsync(new Uri("queue:order_rejected_queue"), context => context.Init<OrderRejected>(new
                {
                    OrderId = context.Instance.CorrelationId,
                    Reason = context.Instance.Reason
                }))
                .TransitionTo(Completed)
                .Finalize()
        );

        
    }

    public State Submitted { get; private set; }

    // public State PaymentProcessing { get; private set; }
    public State PaymentProcessing { get; private set; }
    public State Completed { get; private set; }
    
    public Event<OrderSubmitted> OrderSubmittedEvent { get; private set; }
    public Event<InventoryAllocationConfirmed> InventoryAllocationConfirmedEvent { get; private set; }
    public Event<InventoryAllocationFailed> InventoryAllocationFailedEvent { get; private set; }
    public Event<PaymentProcessed> PaymentProcessedEvent { get; private set; }
}
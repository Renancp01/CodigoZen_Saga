using Confluent.Kafka;
using MassTransit;
using Orders.Consumers;
using Orders.KafkaConsumer;
using Orders.Models;

namespace Orders;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .MongoDbRepository(r =>
                {
                    r.Connection =
                        "mongodb://user:password@localhost:27017";
                    r.DatabaseName = "order_saga_db";
                });

            x.AddConsumer<InventoryAllocationConsumer>();
            x.AddConsumer<PaymentConsumer>();
            x.AddConsumer<OrderAcceptedConsumer>();
            x.AddConsumer<OrderRejectedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("user");
                    h.Password("password");
                });

                cfg.ReceiveEndpoint("order_saga_queue", e => { e.ConfigureSaga<OrderState>(context); });

                cfg.ReceiveEndpoint("inventory_service_queue",
                    e => { e.ConfigureConsumer<InventoryAllocationConsumer>(context); });

                cfg.ReceiveEndpoint("payment_service_queue", e => { e.ConfigureConsumer<PaymentConsumer>(context); });

                cfg.ReceiveEndpoint("order_accepted_queue",
                    e => { e.ConfigureConsumer<OrderAcceptedConsumer>(context); });

                cfg.ReceiveEndpoint("order_rejected_queue",
                    e => { e.ConfigureConsumer<OrderRejectedConsumer>(context); });
            });

            x.AddRider(rider =>
            {
                rider.AddConsumer<KafkaOrderEventConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host("192.168.1.12:9092", _ => { });
                    
                    k.SecurityProtocol = SecurityProtocol.Plaintext;
                    k.TopicEndpoint<KafkaOrderEvent>("order-topic1", "order-group",
                        e =>
                        {
                            e.CreateIfMissing();
                            e.ConfigureConsumer<KafkaOrderEventConsumer>(context);
                        });
                });
            });
        });

        // builder.Services.AddMassTransitHostedService();
        // builder.Services.AddMassTransitHostedService();
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
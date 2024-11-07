using Confluent.Kafka;
using MassTransit;
using MongoDB.Driver;
using Orders.Consumers;
using Orders.KafkaConsumer;
using Orders.Machine;
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

            x.AddConsumer<InventoryAllocationConsumer>(c =>
            {
                c.UseMessageRetry(retryConfigurator => { retryConfigurator.Interval(3, 1000); });
            });

            x.AddConsumer<PaymentConsumer>();
            x.AddConsumer<OrderAcceptedConsumer>();
            x.AddConsumer<OrderRejectedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqp://user:password@localhost:5672/");

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
                x.AddConsumer<KafkaOrderEventConsumer>(config =>
                {
                    config.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5)); // 3 tentativas com intervalo de 5 segundos entre elas
                    });
                });

                rider.UsingKafka((context, k) =>
                {
                    k.Host("192.168.1.12:9092", _ => { });

                    k.SecurityProtocol = SecurityProtocol.Plaintext;
                    k.TopicEndpoint<KafkaOrderEvent>("create-order", "order-group",
                        e =>
                        {
                            // Obtém o IMongoClient do contexto de dependências
                            var mongoClient = context.GetRequiredService<IMongoClient>();

                            // Cria a instância do filtro
                            var inboxFilter = new MongoDbInboxFilter(mongoClient, "inbox_database", "inbox_state");
                            // e.UseConsumeFilter(inboxFilter, context);

                            
                            
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.CreateIfMissing();
                            e.ConfigureConsumer<KafkaOrderEventConsumer>(context);
                        });
                });
            });
        });

        var app = builder.Build();
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
using MassTransit;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class MongoDbInboxFilter : IFilter<ConsumeContext>
{
    private readonly IMongoCollection<ProcessedMessage> _collection;

    public MongoDbInboxFilter(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        _collection = database.GetCollection<ProcessedMessage>(collectionName);
    }

    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        var messageId = context.MessageId?.ToString();

        if (string.IsNullOrEmpty(messageId))
        {
            await next.Send(context);
            return;
        }

        var filter = Builders<ProcessedMessage>.Filter.Eq(pm => pm.MessageId, messageId);

        var processedMessage = await _collection.Find(filter).FirstOrDefaultAsync();

        if (processedMessage != null)
        {
            // Mensagem já processada, ignorar
            return;
        }

        await next.Send(context);

        // Após o processamento bem-sucedido, registre a mensagem como processada
        var newProcessedMessage = new ProcessedMessage
        {
            MessageId = messageId,
            Timestamp = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(newProcessedMessage);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("MongoDbInboxFilter");
    }
}
public class ProcessedMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Campo para o ObjectId do MongoDB

    public string MessageId { get; set; }
    public DateTime Timestamp { get; set; }
}
using MongoDB.Bson;
using MongoDB.Driver;
using Orders.Domain;

namespace Orders.Services;

public class EventService
{
    private readonly IMongoCollection<EventBase> _eventsCollection;
    
    public EventService()
    {
        var client = new MongoClient("mongodb://user:password@localhost:27017");
        var database = client.GetDatabase("EventDatabase");
        _eventsCollection = database.GetCollection<EventBase>("Events");
        //
        // var indexKeysDocument = new BsonDocument("CreatedAt", 1);
        //
        // var indexOptionsDocument = new BsonDocument
        // {
        //     { "expireAfterSeconds", 90 }, // 30 dias em segundos
        //     { "partialFilterExpression", new BsonDocument("Type", "ReceivedOrder") }
        // };

        var command = new BsonDocument
        {
            { "createIndexes", "Events" },
            { "indexes", new BsonArray
                {
                    new BsonDocument
                    {
                        { "key", new BsonDocument { { "CreatedAt", 1 } } },
                        { "name", "TTL_CreatedAt" },
                        { "expireAfterSeconds", 2592000 },
                        { "partialFilterExpression", new BsonDocument { { "Type", "ReceivedOrder" } } }
                    }
                }
            }
        };

        database.RunCommand<BsonDocument>(command);
    }
    
    public void AddEvent(EventBase eventObj)
    {
        _eventsCollection.InsertOne(eventObj);
    }

    public List<EventBase> GetEvents()
    {
        return _eventsCollection.Find(new BsonDocument()).ToList();
    }
}
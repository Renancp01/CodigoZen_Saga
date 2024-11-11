using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Orders.Domain;

public static class MongoGlobalConfig
{
    public static void Configure()
    {
        // Define o modo de representação do Guid para V3
        // BsonDefaults. = GuidRepresentationMode.V3;

        // Registra o serializer padrão para Guid com a representação desejada
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));
        
        // Configura o mapeamento global da classe base EventBase
        BsonClassMap.RegisterClassMap<EventBase>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminatorIsRequired(false); 
            cm.MapMember(c => c.Type).SetElementName("Type"); // Define o nome "Type" globalmente
            cm.SetIgnoreExtraElements(true); // Ignora elementos extras para evitar problemas de deserialização
            cm.MapMember(c => c.CreatedAt).SetElementName("CreatedAt");
        });
        
        BsonClassMap.RegisterClassMap<ReceivedOrderEvent>(cm =>
        {
            cm.AutoMap();
        });


        // Configura mapeamentos adicionais, se necessário, para outras classes
        BsonClassMap.RegisterClassMap<ReceivedOrderEvent>(cm => cm.AutoMap());
    }
}
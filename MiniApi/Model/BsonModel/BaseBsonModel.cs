using MongoDB.Bson;

namespace MiniApi.Model.BsonModel;

public class BaseBsonModel
{
    public ObjectId Id { get; protected set; }
}
using MiniApi.Common.Enum;
using MongoDB.Bson;

namespace MiniApi.Model.BsonModel;

public class Feedback : BaseBsonModel
{
    public WebsiteFeedbackTypeEnum Type { get; private set; }
    public string Content { get; private set; }
    public DateTime ReceiveOn { get; private set; }

    public Feedback(WebsiteFeedbackTypeEnum type, string content, DateTime receiveOn)
    {
        Id = new ObjectId();
        Type = type;
        Content = content;
        ReceiveOn = receiveOn;
    }
}
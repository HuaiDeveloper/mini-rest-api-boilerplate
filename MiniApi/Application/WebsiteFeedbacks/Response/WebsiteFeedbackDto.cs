using MiniApi.Common.Enum;

namespace MiniApi.Application.WebsiteFeedbacks.Response;

public class WebsiteFeedbackDto
{
    public string Id { get; set; }
    public WebsiteFeedbackTypeEnum Type { get; set; }
    public string Content { get; set; }
    public DateTime ReceiveOn { get; set; }
}
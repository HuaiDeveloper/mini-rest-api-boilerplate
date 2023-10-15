using Microsoft.EntityFrameworkCore;
using MiniApi.Application.WebsiteFeedbacks.Request;
using MiniApi.Application.WebsiteFeedbacks.Response;
using MiniApi.Common;
using MiniApi.Common.Enum;
using MiniApi.Common.Exceptions;
using MiniApi.Model.BsonModel;
using MiniApi.Persistence.MongoDBDriver;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MiniApi.Application.WebsiteFeedbacks;

public class WebsiteFeedbackService
{
    private readonly MongoDBContext _mongoDBContext;
    public WebsiteFeedbackService(MongoDBContext mongoDBContext)
    {
        _mongoDBContext = mongoDBContext;
    }

    public Dictionary<string, int> GetWebsiteFeedbackTypes()
    {
        return Enum.GetValues(typeof(WebsiteFeedbackTypeEnum))
            .Cast<WebsiteFeedbackTypeEnum>()
            .ToDictionary(k => k.ToString(), v => (int)v);
    }

    public BasePaginationResponse<List<WebsiteFeedbackDto>> SearchWebsiteFeedbacksAsync(BasePaginationRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var websiteFeedbackQuery = _mongoDBContext.Feedbacks.AsQueryable();
            
        var websiteFeedbacks = websiteFeedbackQuery
            .OrderByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToList();

        var result = websiteFeedbacks
            .Where(x => x != null)
            .Select(x => new WebsiteFeedbackDto()
            {
                Id = x.Id.ToString() ?? string.Empty,
                Type = x.Type,
                Content = x.Content.ToString() ?? string.Empty,
                ReceiveOn = x.ReceiveOn
            })
            .ToList();

        var totalCount = websiteFeedbackQuery.Count();

        return new BasePaginationResponse<List<WebsiteFeedbackDto>>()
        {
            Data = result,
            TotalCount = totalCount
        };
    }
    
    public async Task<string> CreateWebsiteFeedbackAsync(CreateWebsiteFeedbackRequest request)
    {
        var receiveOn = DateTime.UtcNow;
        
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var websiteFeedback = new Feedback(request.Type, request.Content, receiveOn);
        
        await _mongoDBContext.Feedbacks.InsertOneAsync(websiteFeedback);

        return "Success feedback";
    }
    
    public async Task<string> DeleteWebsiteFeedbackAsync(string id)
    {
        var tryParseObjectId = ObjectId.TryParse(id, out var requestObjectId);
        if (tryParseObjectId == false)
            throw new NotFoundException($"Not found id: {id}");
        
        var filter = Builders<Feedback>.Filter.Where(f => f.Id == requestObjectId);
        
        await _mongoDBContext.Feedbacks.DeleteOneAsync(filter);

        return "Successfully deleted";
    }
}
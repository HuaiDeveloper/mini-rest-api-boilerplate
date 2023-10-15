using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.WebsiteFeedbacks.Request;
using MiniApi.Common;

namespace MiniApi.Application.WebsiteFeedbacks;

public static class WebsiteFeedbackEndpoint
{
    internal static IEndpointRouteBuilder MapWebsiteFeedbackEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/feedback-types", (
                [FromServices] WebsiteFeedbackService websiteFeedbackService) 
                => websiteFeedbackService.GetWebsiteFeedbackTypes())
            .RequireAuthorization()
            .WithName("GetWebsiteFeedbackTypes")
            .WithOpenApi();

        endpointRouteBuilder
            .MapPost("/feedback", async (
                [FromBody] CreateWebsiteFeedbackRequest request,
                [FromServices] WebsiteFeedbackService websiteFeedbackService)
                => await websiteFeedbackService.CreateWebsiteFeedbackAsync(request))
            .RequireAuthorization()
            .WithName("CreateWebsiteFeedback")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapGet("/feedback", (
                [AsParameters] BasePaginationRequest queryString,
                [FromServices] WebsiteFeedbackService websiteFeedbackService)
                => websiteFeedbackService.SearchWebsiteFeedbacksAsync(queryString))
            .RequireAuthorization(AuthRole.Admin)
            .WithName("SearchWebsiteFeedbacks")
            .WithOpenApi();

        endpointRouteBuilder
            .MapDelete("/feedback/{id}", async (
                string id,
                [FromServices] WebsiteFeedbackService websiteFeedbackService)
                => await websiteFeedbackService.DeleteWebsiteFeedbackAsync(id))
            .RequireAuthorization(AuthRole.Admin)
            .WithName("DeleteWebsiteFeedback")
            .WithOpenApi();
        
        return endpointRouteBuilder;
    }
}
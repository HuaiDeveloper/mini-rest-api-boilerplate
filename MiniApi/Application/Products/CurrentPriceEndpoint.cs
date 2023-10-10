using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.Products.Request;

namespace MiniApi.Application.Products;

internal static class CurrentPriceEndpoint
{
    internal static IEndpointRouteBuilder MapCurrentPriceEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/current-price", async (
                [FromBody] CreateCurrentPriceRequest request,
                [FromServices] CurrentPriceService currentPriceService) => await currentPriceService.CreateCurrentPriceAsync(request))
            .RequireAuthorization()
            .WithName("CreateCurrentPrice")
            .WithOpenApi();

        endpointRouteBuilder
            .MapPut("/current-price", async (
                [FromBody] UpdateCurrentPriceRequest request,
                [FromServices] CurrentPriceService currentPriceService) => await currentPriceService.UpdateCurrentPriceAsync(request))
            .RequireAuthorization()
            .WithName("UpdateCurrentPrice")
            .WithOpenApi();
        
        return endpointRouteBuilder;
    }
}
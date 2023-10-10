using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.Products.Request;
using MiniApi.Common;

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
        
        endpointRouteBuilder
            .MapGet("/product-latest-prices", async (
                [AsParameters] BasePaginationRequest queryString,
                [FromServices] CurrentPriceService currentPriceService) => await currentPriceService.SearchProductLatestPricesAsync(queryString))
            .RequireAuthorization()
            .WithName("SearchProductLatestPrices")
            .WithOpenApi();
        
        return endpointRouteBuilder;
    }
}
using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.SerialNumberCodes.Request;
using MiniApi.Common;

namespace MiniApi.Application.SerialNumberCodes;

public static class SerialNumberCodeEndpoint
{
    internal static IEndpointRouteBuilder MapSerialNumberCodeEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/generate-serial-number-code", async (
                    [FromBody] GenerateSerialNumberCodeRequest request, 
                    [FromServices] SerialNumberCodeService serialNumberCodeService) 
                => await serialNumberCodeService.GenerateSerialNumberCodeAsync(request))
            .RequireAuthorization()
            .WithName("GenerateSerialNumberCode")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapPost("/regenerate-serial-number-code", async (
                    [FromBody] RegenerateSerialNumberCodeRequest request,
                    [FromServices] SerialNumberCodeService serialNumberCodeService) 
                => await serialNumberCodeService.RegenerateSerialNumberCodeAsync(request))
            .RequireAuthorization()
            .WithName("RegenerateSerialNumberCode")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapGet("/serial-number-code", async (
                    [AsParameters] BasePaginationRequest queryString, 
                    [FromServices] SerialNumberCodeService serialNumberCodeService) 
                => await serialNumberCodeService.SearchSerialNumberCodesAsync(queryString))
            .RequireAuthorization()
            .WithName("SearchSerialNumberCodes")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapDelete("/clear-serial-number-code", async (
                    [FromServices] SerialNumberCodeService serialNumberCodeService) 
                => await serialNumberCodeService.ClearSerialNumberCodesAsync())
            .RequireAuthorization()
            .WithName("ClearSerialNumberCodes")
            .WithOpenApi();
        
        return endpointRouteBuilder;
    }
}
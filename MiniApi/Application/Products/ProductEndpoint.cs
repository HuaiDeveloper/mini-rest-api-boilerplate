using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.Products.Request;
using MiniApi.Common;

namespace MiniApi.Application.Products;

internal static class ProductEndpoint
{
    internal static IEndpointRouteBuilder MapProductEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/product/{id}", async (
                long id,
                [FromServices] ProductService productService) => await productService.GetProductAsync(id))
            .RequireAuthorization()
            .WithName("GetProduct")
            .WithOpenApi();

        endpointRouteBuilder
            .MapGet("/product", async (
                [AsParameters] BasePaginationRequest queryString,
                [FromServices] ProductService productService) => await productService.SearchProductsAsync(queryString))
            .RequireAuthorization()
            .WithName("SearchProducts")
            .WithOpenApi();

        endpointRouteBuilder
            .MapPost("/product", async (
                [FromBody] CreateProductRequest request,
                [FromServices] ProductService productService) => await productService.CreateProductAsync(request))
            .RequireAuthorization()
            .WithName("CreateProduct")
            .WithOpenApi();

        endpointRouteBuilder
            .MapPut("/product", async (
                [FromBody] UpdateProductRequest request,
                [FromServices] ProductService productService) => await productService.UpdateProductAsync(request))
            .RequireAuthorization()
            .WithName("UpdateProduct")
            .WithOpenApi();

        endpointRouteBuilder
            .MapDelete("/product/{id}", async (
                long id,
                [FromServices] ProductService productService) => await productService.DeleteProductAsync(id))
            .RequireAuthorization()
            .WithName("DeleteProduct")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}

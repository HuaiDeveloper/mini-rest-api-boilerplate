using Microsoft.AspNetCore.Mvc;
using MiniApi.Applicatoin.Products.Request;

namespace MiniApi.Applicatoin.Products
{
    internal static class ProductEndpoint
    {
        internal static IEndpointRouteBuilder AddProductEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder
                .MapGet("/product/{id}", async (
                    Guid id,
                    [FromServices] ProductHandle productHandle) =>
                {
                    return await productHandle.GetProductAsync(id);
                })
                .WithName("GetProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapGet("/product", async (
                    [FromQuery] int page,
                    [FromQuery] int size,
                    [FromServices] ProductHandle productHandle) =>
                {
                    return await productHandle.SearchProductsAsync(page, size);
                })
                .WithName("SearchProducts")
                .WithOpenApi();

            endpointRouteBuilder
                .MapPost("/product", async (
                    [FromBody] CreateProductRequest request,
                    [FromServices] ProductHandle productHandle) =>
                {
                    return await productHandle.CreateProductAsync(request);
                })
                .WithName("CreateProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapPut("/product", async (
                    [FromBody] UpdateProductRequest request,
                    [FromServices] ProductHandle productHandle) =>
                {
                    return await productHandle.UpdateProductAsync(request);
                })
                .WithName("UpdateProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapDelete("/product/{id}", async (
                    Guid id,
                    [FromServices] ProductHandle productHandle) =>
                {
                    return await productHandle.DeleteProductAsync(id);
                })
                .WithName("DeleteProduct")
                .WithOpenApi();

            return endpointRouteBuilder;
        }
    }
}

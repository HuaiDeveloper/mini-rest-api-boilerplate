using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.Products.Request;

namespace MiniApi.Application.Products
{
    internal static class ProductEndpoint
    {
        internal static IEndpointRouteBuilder MapProductEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder
                .MapGet("/product/{id}", async (
                    Guid id,
                    [FromServices] ProductService productService) => await productService.GetProductAsync(id))
                .WithName("GetProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapGet("/product", async (
                    [FromQuery] int page,
                    [FromQuery] int size,
                    [FromServices] ProductService productService) => await productService.SearchProductsAsync(page, size))
                .WithName("SearchProducts")
                .WithOpenApi();

            endpointRouteBuilder
                .MapPost("/product", async (
                    [FromBody] CreateProductRequest request,
                    [FromServices] ProductService productService) => await productService.CreateProductAsync(request))
                .WithName("CreateProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapPut("/product", async (
                    [FromBody] UpdateProductRequest request,
                    [FromServices] ProductService productService) => await productService.UpdateProductAsync(request))
                .WithName("UpdateProduct")
                .WithOpenApi();

            endpointRouteBuilder
                .MapDelete("/product/{id}", async (
                    Guid id,
                    [FromServices] ProductService productService) => await productService.DeleteProductAsync(id))
                .WithName("DeleteProduct")
                .WithOpenApi();

            return endpointRouteBuilder;
        }
    }
}

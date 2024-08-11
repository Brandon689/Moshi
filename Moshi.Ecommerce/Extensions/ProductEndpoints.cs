using Moshi.Ecommerce.Models;
using Moshi.Ecommerce.Services;

namespace Moshi.Ecommerce.Extensions;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products").WithTags("Products");

        group.MapGet("/", async (ProductService productService) =>
        {
            var products = await productService.GetProductsAsync();
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithOpenApi();

        group.MapPost("/", async (Product product, ProductService productService) =>
        {
            await productService.AddProductAsync(product);
            return Results.Created($"/products/{product.Id}", product);
        })
        .WithName("AddProduct")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, Product product, ProductService productService) =>
        {
            await productService.UpdateProductAsync(id, product);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, ProductService productService) =>
        {
            await productService.DeleteProductAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();

        return endpoints;
    }
}
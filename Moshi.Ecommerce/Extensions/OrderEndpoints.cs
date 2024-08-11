using Moshi.Ecommerce.Models;
using Moshi.Ecommerce.Services;

namespace Moshi.Ecommerce.Extensions;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", async (OrderService orderService) =>
        {
            var orders = await orderService.GetOrdersAsync();
            return Results.Ok(orders);
        })
        .WithName("GetOrders")
        .WithOpenApi();

        group.MapGet("/with-items", async (OrderService orderService) =>
        {
            var orders = await orderService.GetOrdersWithItemsAsync();
            return Results.Ok(orders);
        })
        .WithName("GetOrdersWithItems")
        .WithOpenApi();

        group.MapPost("/", async (Order order, OrderService orderService) =>
        {
            await orderService.AddOrderAsync(order);
            return Results.Created($"/orders/{order.Id}", order);
        })
        .WithName("AddOrder")
        .WithOpenApi();

        group.MapPost("/items", async (OrderItem orderItem, OrderService orderService) =>
        {
            await orderService.AddOrderItemAsync(orderItem);
            return Results.Created($"/orderitems/{orderItem.Id}", orderItem);
        })
        .WithName("AddOrderItem")
        .WithOpenApi();

        return endpoints;
    }
}
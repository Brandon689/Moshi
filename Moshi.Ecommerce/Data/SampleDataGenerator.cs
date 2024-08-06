using Bogus;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Data;

public class SampleDataGenerator
{
    public List<Product> GenerateProducts(int count)
    {
        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => double.Parse(f.Commerce.Price(10, 100)));

        return productFaker.Generate(count);
    }

    public List<Order> GenerateOrders(int count, List<Product> products)
    {
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.OrderDate, f => f.Date.Past(1));

        var orderItemFaker = new Faker<OrderItem>()
            .RuleFor(oi => oi.ProductId, f => f.PickRandom(products).Id)
            .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 5));

        var orders = orderFaker.Generate(count);
        foreach (var order in orders)
        {
            var orderItems = orderItemFaker.Generate(new Faker().Random.Int(1, 5));
            order.OrderItems.AddRange(orderItems);
        }

        return orders;
    }
}
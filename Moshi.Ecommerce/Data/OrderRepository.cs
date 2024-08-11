using Microsoft.Data.Sqlite;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Data;

public class OrderRepository
{
    private readonly SqliteConnection _connection;

    public OrderRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        var orders = new List<Order>();
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM Orders";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var order = new Order
            {
                Id = reader.GetInt32(0),
                OrderDate = DateTime.Parse(reader.GetString(1))
            };
            orders.Add(order);
        }
        _connection.Close();
        return orders;
    }

    public async Task<List<Order>> GetOrdersWithItemsAsync()
    {
        var orders = new Dictionary<int, Order>();
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT o.Id AS OrderId, o.OrderDate, 
                    oi.Id AS OrderItemId, oi.ProductId, oi.Quantity, 
                    p.Id AS ProductId, p.Name, p.Price
            FROM Orders o
            LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
            LEFT JOIN Products p ON oi.ProductId = p.Id";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var orderId = reader.GetInt32(0);
            if (!orders.ContainsKey(orderId))
            {
                orders[orderId] = new Order
                {
                    Id = orderId,
                    OrderDate = DateTime.Parse(reader.GetString(1)),
                    OrderItems = new List<OrderItem>()
                };
            }

            if (!reader.IsDBNull(2))
            {
                var orderItem = new OrderItem
                {
                    Id = reader.GetInt32(2),
                    OrderId = orderId,
                    ProductId = reader.GetInt32(3),
                    Quantity = reader.GetInt32(4),
                    Product = new Product
                    {
                        Id = reader.GetInt32(5),
                        Name = reader.GetString(6),
                        Price = reader.GetDouble(7)
                    }
                };
                orders[orderId].OrderItems.Add(orderItem);
            }
        }
        _connection.Close();
        return orders.Values.ToList();
    }

    public async Task AddOrderAsync(Order order)
    {
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO Orders (OrderDate) VALUES (@orderDate)";
        command.Parameters.AddWithValue("@orderDate", order.OrderDate.ToString("o"));
        await command.ExecuteNonQueryAsync();

        // Retrieve the last inserted row ID
        command.CommandText = "SELECT last_insert_rowid()";
        order.Id = Convert.ToInt32(await command.ExecuteScalarAsync());

        _connection.Close();
    }

    public async Task AddOrderItemAsync(OrderItem orderItem)
    {
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@orderId, @productId, @quantity)";
        command.Parameters.AddWithValue("@orderId", orderItem.OrderId);
        command.Parameters.AddWithValue("@productId", orderItem.ProductId);
        command.Parameters.AddWithValue("@quantity", orderItem.Quantity);
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }

    public async Task UpdateOrderPaymentStatusAsync(int orderId, string paymentStatus)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE Orders SET PaymentStatus = @PaymentStatus WHERE Id = @Id";
        command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
        command.Parameters.AddWithValue("@Id", orderId);
        await command.ExecuteNonQueryAsync();
    }
}
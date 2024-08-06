using Microsoft.Data.Sqlite;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Data;

public class ProductRepository
{
    private readonly SqliteConnection _connection;

    public ProductRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        var products = new List<Product>();
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM Products";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDouble(2)
            });
        }
        _connection.Close();
        return products;
    }

    public async Task AddProductAsync(Product product)
    {
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO Products (Name, Price) VALUES (@name, @price)";
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@price", product.Price);
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }

    public async Task UpdateProductAsync(int id, Product product)
    {
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE Products SET Name = @name, Price = @price WHERE Id = @id";
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }

    public async Task DeleteProductAsync(int id)
    {
        _connection.Open();
        using var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }
}
using Microsoft.Data.Sqlite;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Data
{
    public class DatabaseInitializer
    {
        private readonly SqliteConnection _connection;
        private readonly SampleDataGenerator _sampleDataGenerator;

        public DatabaseInitializer(SqliteConnection connection, SampleDataGenerator sampleDataGenerator)
        {
            _connection = connection;
            _sampleDataGenerator = sampleDataGenerator;
        }

        public void Initialize()
        {
            _connection.Open();
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderDate TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                );
            ";
            command.ExecuteNonQuery();

            // Check if sample data already exists
            command.CommandText = "SELECT COUNT(*) FROM Products";
            var productCount = (long)command.ExecuteScalar();
            if (productCount == 0)
            {
                var products = _sampleDataGenerator.GenerateProducts(10);
                foreach (var product in products)
                {
                    command.CommandText = "INSERT INTO Products (Name, Price) VALUES (@name, @price)";
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }

            command.CommandText = "SELECT COUNT(*) FROM Orders";
            var orderCount = (long)command.ExecuteScalar();
            if (orderCount == 0)
            {
                command.CommandText = "SELECT Id FROM Products";
                var productIds = new List<int>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productIds.Add(reader.GetInt32(0));
                    }
                }

                var products = productIds.Select(id => new Product { Id = id }).ToList();
                var orders = _sampleDataGenerator.GenerateOrders(5, products);
                foreach (var order in orders)
                {
                    command.CommandText = "INSERT INTO Orders (OrderDate) VALUES (@orderDate)";
                    command.Parameters.AddWithValue("@orderDate", order.OrderDate.ToString("o"));
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT last_insert_rowid()";
                    var orderId = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();

                    foreach (var orderItem in order.OrderItems)
                    {
                        command.CommandText = "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@orderId, @productId, @quantity)";
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@productId", orderItem.ProductId);
                        command.Parameters.AddWithValue("@quantity", orderItem.Quantity);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            }

            _connection.Close();
        }
    }
}

using Microsoft.Data.Sqlite;
using Moshi.Ecommerce.Data;
using Moshi.Ecommerce.Extensions;
using Moshi.Ecommerce.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Initialize SQLite connection
string connectionString = "Data Source=ecommerce.db";
var connection = new SqliteConnection(connectionString);
builder.Services.AddSingleton(connection);
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<SampleDataGenerator>();
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Initialize Database
var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
dbInitializer.Initialize();

// Map Endpoints
app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();

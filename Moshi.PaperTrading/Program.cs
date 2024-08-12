using Moshi.PaperTrading.Services;
using PaperTradingApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PolygonOptions
builder.Services.Configure<PolygonOptions>(options =>
{
    options.ApiKey = builder.Configuration["PolygonApiKey"] ??
                     builder.Configuration["Polygon:ApiKey"];
});

builder.Services.AddSingleton<PolygonService>();

// Uncomment this line for debugging
// Console.WriteLine($"API Key: {builder.Configuration["PolygonApiKey"] ?? builder.Configuration["Polygon:ApiKey"]}");

builder.Services.AddSingleton<IStockService, StockService>();
builder.Services.AddSingleton<IPortfolioService, PortfolioService>();
builder.Services.AddSingleton<ITradeService, TradeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
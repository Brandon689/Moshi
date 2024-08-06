using Microsoft.Data.Sqlite;
using Moshi.Forums.Data;
using Moshi.Forums.Extensions;
using Moshi.Forums.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your services
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<ThreadRepository>();
builder.Services.AddSingleton<PostRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ForumRepository>();

builder.Services.AddSingleton<ThreadService>();
builder.Services.AddSingleton<PostService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ForumService>();

builder.Services.AddSingleton<SampleDataGenerator>();

// Configure SQLite
builder.Services.AddSingleton(new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapThreadEndpoints();
app.MapPostEndpoints();
app.MapUserEndpoints();
app.MapForumEndpoints();

// Initialize database and generate sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeAsync();

    var sampleDataGenerator = services.GetRequiredService<SampleDataGenerator>();
    await sampleDataGenerator.GenerateSampleDataAsync();
}

app.Run();

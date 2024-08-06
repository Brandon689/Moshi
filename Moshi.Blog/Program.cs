using Microsoft.OpenApi.Models;
using Moshi.Blog.Data;
using Moshi.Blog.Extensions;
using Moshi.Blog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Moshi Blog API", Version = "v1" });
});

// Configure SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=blog.db";

// Register services
builder.Services.AddSingleton<DatabaseInitializer>(sp => new DatabaseInitializer(connectionString));
builder.Services.AddSingleton<UserRepository>(sp => new UserRepository(connectionString));
builder.Services.AddSingleton<PostRepository>(sp => new PostRepository(connectionString));
builder.Services.AddSingleton<CommentRepository>(sp => new CommentRepository(connectionString));
builder.Services.AddSingleton<CategoryRepository>(sp => new CategoryRepository(connectionString));
builder.Services.AddSingleton<SampleDataGenerator>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Initialize the database and generate sample data
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        dbInitializer.Initialize();

        var sampleDataGenerator = scope.ServiceProvider.GetRequiredService<SampleDataGenerator>();
        await sampleDataGenerator.GenerateSampleData();
    }
}

app.UseHttpsRedirection();

// Map API endpoints
app.MapUserEndpoints();
app.MapPostEndpoints();
app.MapCommentEndpoints();

app.Run();

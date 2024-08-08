using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Moshi.Forums.Data;
using Moshi.Forums.Extensions;
using Moshi.Forums.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<ThreadRepository>();
builder.Services.AddSingleton<PostRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ForumRepository>();

builder.Services.AddSingleton<ThreadService>();
builder.Services.AddSingleton<PostService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ForumService>();

// New repositories
builder.Services.AddSingleton<SubscriptionRepository>();
builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddSingleton<UserRoleRepository>();

// New services
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<ProfileService>();
builder.Services.AddSingleton<SubscriptionService>();
builder.Services.AddSingleton<ModerationService>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddSingleton<SampleDataGenerator>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// Map endpoints
app.MapThreadEndpoints();
app.MapPostEndpoints();
app.MapUserEndpoints();
app.MapForumEndpoints();
app.MapSubscriptionEndpoints();
app.MapNotificationEndpoints();
app.MapProfileEndpoints();
app.MapSearchEndpoints();
app.MapModerationEndpoints();
app.MapAuthEndpoints();
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

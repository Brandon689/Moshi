using Moshi.MMO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:8080")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.MapHub<GameHub>("/gamehub");

app.Run();
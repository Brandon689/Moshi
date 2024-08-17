using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddScoped<SubtitleService>();
builder.Services.AddScoped<SubtitleParserService>();
builder.Services.AddScoped<MoviesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SubtitleRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<DatabaseInitializer>();
    dbInitializer.Initialize();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
using Moshi.MediaWiki.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register your services
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new UserService(connectionString));
builder.Services.AddSingleton(new PageService(connectionString));
builder.Services.AddSingleton(new RevisionService(connectionString));
builder.Services.AddSingleton(new CategoryService(connectionString));
builder.Services.AddSingleton(new FileUploadService(connectionString));
builder.Services.AddSingleton(new LinkService(connectionString));
builder.Services.AddSingleton(new UserGroupService(connectionString));
builder.Services.AddSingleton(new PageProtectionService(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
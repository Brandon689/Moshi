using Supabase;
using SupaBaseDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var url = configuration["Supabase:Url"];
    var key = configuration["Supabase:Key"];
    return new Client(url, key);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Define API endpoints
app.MapGet("/users", async (Client supabase) =>
{
    var response = await supabase
        .From<User>()
        .Get();
    return Results.Ok(response.Models);
});

app.MapGet("/users/{id}", async (int id, Client supabase) =>
{
    var response = await supabase
        .From<User>()
        .Where(u => u.Id == id)
        .Get();

    if (response.Models.Count == 0)
        return Results.NotFound();

    return Results.Ok(response.Models[0]);
});

app.MapPost("/users", async (User user, Client supabase) =>
{
    var response = await supabase
        .From<User>()
        .Insert(user);

    return Results.Created($"/users/{response.Models[0].Id}", response.Models[0]);
});

app.MapPut("/users/{id}", async (int id, User user, Client supabase) =>
{
    user.Id = id; // Ensure the ID is set correctly
    var response = await supabase
        .From<User>()
        .Update(user);

    if (!response.Models.Any())
        return Results.NotFound();

    return Results.Ok(response.Models.First());
});


app.MapDelete("/users/{id}", async (int id, Client supabase) =>
{
    try
    {
        await supabase
            .From<User>()
            .Where(u => u.Id == id)
            .Delete();

        // If we reach this point, the deletion was successful
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        // Log the exception if needed
        return Results.NotFound();
    }
});





app.Run();
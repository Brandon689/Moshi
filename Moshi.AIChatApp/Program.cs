using Mscc.GenerativeAI.Web;

var builder = WebApplication.CreateBuilder(args);
// this library  Mscc.GenerativeAI.Web has not implemented chatsessions in asp, do not use
// Add Gemini AI service
builder.Services.AddGenerativeAI(builder.Configuration.GetSection("Gemini"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.MapGet("/", async (IGenerativeModelService service) =>
{
    var model = service.CreateInstance();
    var result = await model.GenerateContent("Write about the history of Mauritius.");
    return result.Text;
})
.WithOpenApi();


app.Run();
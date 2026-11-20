using EventsApi.Endpoints;
using EventsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEventService, EventService>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    options.IncludeXmlComments(filePath);
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapEventEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

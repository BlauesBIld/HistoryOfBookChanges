using System.Text.Json.Serialization;
using TechnicalTask.Common;
using TechnicalTask.Repositories;
using TechnicalTask.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await SeedData.InitializeAsync(app.Services);

app.Run();
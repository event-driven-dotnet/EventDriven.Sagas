using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SagaConfigService.Configuration;
using SagaConfigService.DTO;
using SagaConfigService.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
builder.Services.Configure<SagaConfigDatabaseSettings>(
    builder.Configuration.GetSection(nameof(SagaConfigDatabaseSettings)));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<SagaConfigDatabaseSettings>>().Value);

// Registrations
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<SagaConfigDatabaseSettings>();
    var client = new MongoClient(settings.ConnectionString);
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<SagaConfiguration>(settings.SagaConfigCollectionName);
});
builder.Services.AddSingleton<IDocumentRepository<SagaConfiguration>, DocumentRepository<SagaConfiguration>>();
builder.Services.AddSingleton<ISagaConfigRepository, SagaConfigRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

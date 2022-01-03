using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Sagas.CreateOrder;
using OrderService.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

// Configuration
builder.Services.Configure<OrderServiceSettings>(
    builder.Configuration.GetSection(nameof(OrderServiceSettings)));
builder.Services.Configure<OrderDatabaseSettings>(
    builder.Configuration.GetSection(nameof(OrderDatabaseSettings)));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<OrderDatabaseSettings>>().Value);
builder.Services.Configure<SagaConfigDatabaseSettings>(
    builder.Configuration.GetSection(nameof(SagaConfigDatabaseSettings)));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<SagaConfigDatabaseSettings>>().Value);

// Database Registrations
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<OrderDatabaseSettings>();
    var client = new MongoClient(settings.ConnectionString);
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Order>(settings.OrderCollectionName);
});
builder.Services.AddSingleton<IDocumentRepository<Order>, DocumentRepository<Order>>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<SagaConfigDatabaseSettings>();
    var client = new MongoClient(settings.ConnectionString);
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<SagaConfiguration>(settings.SagaConfigCollectionName);
});
builder.Services.AddSingleton<IDocumentRepository<SagaConfiguration>, DocumentRepository<SagaConfiguration>>();
builder.Services.AddSingleton<ISagaConfigRepository, SagaConfigRepository>();

// Saga Registrations
builder.Services.AddSingleton<OrderCommandHandler>();
builder.Services.AddSingleton<ISagaCommandDispatcher, CreateOrderCommandDispatcher>();
builder.Services.AddSingleton<ICommandResultEvaluator<OrderState, OrderState>, SetOrderStateResultEvaluator>();
var orderServiceSettings = new OrderServiceSettings();
builder.Configuration.GetSection(nameof(OrderServiceSettings)).Bind(orderServiceSettings);
builder.Services.AddSaga<CreateOrderSaga>(orderServiceSettings.CreateOrderSagaConfigId);

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

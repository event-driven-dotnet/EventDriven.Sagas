using EventDriven.DDD.Abstractions.Commands;
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
// ICommandHandler<Order, CreateOrder>: CreateOrderCommandHandler
builder.Services.AddSingleton<ICommandHandler<Order, CreateOrder>, CreateOrderCommandHandler>(sp =>
{
    var repo = sp.GetRequiredService<IOrderRepository>();
    var saga = sp.GetRequiredService<CreateOrderSaga>();
    var logger = sp.GetRequiredService<ILogger<CreateOrderCommandHandler>>();
    var handler = new CreateOrderCommandHandler(repo, saga, logger);
    return handler;
});

// -> Saga: CreateOrderSaga
var orderServiceSettings = new OrderServiceSettings();
builder.Configuration.GetSection(nameof(OrderServiceSettings)).Bind(orderServiceSettings);
builder.Services.AddSaga<CreateOrderSaga>(orderServiceSettings.CreateOrderSagaConfigId);

//    -> ISagaCommandDispatcher: OrderCommandDispatcher
builder.Services.AddSingleton<ISagaCommandDispatcher, OrderCommandDispatcher>();

//        -> ICommandHandler<Order, SetOrderStatePending>: SetOrderStateCommandHandler
builder.Services.AddSingleton<ICommandHandler<Order, SetOrderStatePending>, SetOrderStateCommandHandler>(sp =>
{
    var repo = sp.GetRequiredService<IOrderRepository>();
    var saga = sp.GetRequiredService<CreateOrderSaga>();
    var logger = sp.GetRequiredService<ILogger<SetOrderStateCommandHandler>>();
    var handler = new SetOrderStateCommandHandler(repo, saga, logger);
    return handler;
});

//            -> ICommandResultProcessor<Order>: CreateOrderSaga
builder.Services.AddSingleton<ICommandResultProcessor<Order>, CreateOrderSaga>();

builder.Services.AddSingleton<ICommandResultEvaluator<OrderState, OrderState>, SetOrderStateResultEvaluator>();


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

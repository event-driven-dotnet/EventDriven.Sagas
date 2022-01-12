using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Configuration.Repositories;
using EventDriven.Sagas.DependencyInjection;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Repositories;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands.Dispatchers;
using OrderService.Domain.OrderAggregate.Commands.Evaluators;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;
using OrderService.Domain.OrderAggregate.Sagas;
using OrderService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

// Configuration
builder.Services.AddAppSettings<SagaConfigSettings>(builder.Configuration);

// Database registrations
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(
    builder.Configuration);
builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(
    builder.Configuration);
builder.Services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(
    builder.Configuration);

// Saga registration
builder.Services.AddSaga<CreateOrderSaga, Order, SetOrderStatePending,
    OrderCommandDispatcher, SetOrderStateResultEvaluator, 
    SagaConfigRepository, SagaSnapshotRepository, SagaConfigSettings>(
    builder.Configuration);

// Command handler registrations
builder.Services.AddCommandHandlers();

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

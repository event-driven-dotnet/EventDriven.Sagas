using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Sagas.CreateOrder;
using OrderService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

// Configuration
builder.Services.AddAppSettings<SagaConfigSettings>(builder.Configuration);

// Database Registrations
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<ISagaConfigRepository, SagaConfigRepository>();

// Saga Registrations
builder.Services.AddSaga<CreateOrderSaga, OrderCommandDispatcher,
    SagaConfigRepository, SetOrderStateResultEvaluator,
    SagaConfigSettings>(builder.Configuration);

// Saga command dispatcher, processor, evaluator
builder.Services.AddSingleton<ISagaCommandDispatcher, OrderCommandDispatcher>();
builder.Services.AddSingleton<ICommandResultProcessor<Order>, CreateOrderSaga>();
builder.Services.AddSingleton<ICommandResultEvaluator<OrderState, OrderState>, SetOrderStateResultEvaluator>();

// Saga command handlers
builder.Services.AddSingleton<ICommandHandler<Order, CreateOrder>, CreateOrderCommandHandler>();
builder.Services.AddSingleton<ICommandHandler<Order, SetOrderStatePending>, SetOrderStateCommandHandler>();

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

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
builder.Services.AddAppSettings<OrderServiceSettings>(builder.Configuration);

// Database Registrations
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
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

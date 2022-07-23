using Common.Behaviors;
using Common.Integration.Events;
using EventDriven.CQRS.Abstractions.DependencyInjection;
using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Configuration.Mongo.Repositories;
using EventDriven.Sagas.DependencyInjection;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Mongo.Repositories;
using MediatR;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Integration.Handlers;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;
using OrderService.Sagas.CreateOrder.Dispatchers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add database settings
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(builder.Configuration);

// Add command and query handlers
builder.Services.AddHandlers(typeof(Program));

// Add behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// App settings
builder.Services.AddAppSettings<SagaConfigSettings>(builder.Configuration);

// Sagas
builder.Services.AddSaga<CreateOrderSaga, CreateOrderSagaCommandDispatcher,
    SagaConfigRepository, SagaSnapshotRepository, SagaConfigSettings>(
    builder.Configuration);

// Event Bus and event handlers
builder.Services.AddDaprEventBus(builder.Configuration, true);
builder.Services.AddDaprMongoEventCache(builder.Configuration);
builder.Services.AddSingleton<CustomerCreditReserveFulfilledEventHandler>();
builder.Services.AddSingleton<CustomerCreditReleaseFulfilledEventHandler>();
builder.Services.AddSingleton<ProductInventoryReserveFulfilledEventHandler>();
builder.Services.AddSingleton<ProductInventoryReleaseFulfilledEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// Map Dapr Event Bus subscribers
app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSubscribeHandler();
    endpoints.MapDaprEventBus(eventBus =>
    {
        var customerCreditReservedEventHandler = app.Services.GetRequiredService<CustomerCreditReserveFulfilledEventHandler>();
        var customerCreditReleasedEventHandler = app.Services.GetRequiredService<CustomerCreditReleaseFulfilledEventHandler>();
        var productInventoryReservedEventHandler = app.Services.GetRequiredService<ProductInventoryReserveFulfilledEventHandler>();
        var productInventoryReleasedEventHandler = app.Services.GetRequiredService<ProductInventoryReleaseFulfilledEventHandler>();
        eventBus?.Subscribe(customerCreditReservedEventHandler, nameof(CustomerCreditReserveFulfilled), "v1");
        eventBus?.Subscribe(customerCreditReleasedEventHandler, nameof(CustomerCreditReleaseFulfilled), "v1");
        eventBus?.Subscribe(productInventoryReservedEventHandler, nameof(ProductInventoryReserveFulfilled), "v1");
        eventBus?.Subscribe(productInventoryReleasedEventHandler, nameof(ProductInventoryReleaseFulfilled), "v1");
    });
});

app.Run();
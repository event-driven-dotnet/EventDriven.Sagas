using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Configuration.Mongo.Repositories;
using EventDriven.Sagas.DependencyInjection;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Mongo.Repositories;
using Integration.Events;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Integration.Handlers;
using OrderService.Sagas;
using OrderService.Sagas.Dispatchers;
using OrderService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add database settings
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(builder.Configuration);
builder.Services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(builder.Configuration);

// Add command handlers
builder.Services.AddCommandHandlers();

// Add saga
builder.Services.AddAppSettings<SagaConfigSettings>(builder.Configuration);
builder.Services.AddSaga<CreateOrderSaga, CreateOrderSagaCommandDispatcher,
    SagaConfigRepository, SagaSnapshotRepository, SagaConfigSettings>(
    builder.Configuration);

// Add Dapr Event Bus and event handler
builder.Services.AddDaprEventBus(builder.Configuration, true);
builder.Services.AddSingleton<CustomerCreditReserveFulfilledEventHandler>();

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
        eventBus.Subscribe(customerCreditReservedEventHandler, nameof(CustomerCreditReserveFulfilled), "v1");
    });
});

app.Run();
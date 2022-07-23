using Common.Behaviors;
using Common.Integration.Events;
using EventDriven.CQRS.Abstractions.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using InventoryService.Configuration;
using InventoryService.Domain.InventoryAggregate;
using InventoryService.Integration.Handlers;
using InventoryService.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add database settings
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
builder.Services.AddMongoDbSettings<InventoryDatabaseSettings, Inventory>(builder.Configuration);

// Add command and query handlers
builder.Services.AddHandlers(typeof(Program));

// Add behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Add Dapr Event Bus and event handler
builder.Services.AddDaprEventBus(builder.Configuration, true);
builder.Services.AddDaprMongoEventCache(builder.Configuration);
builder.Services.AddSingleton<ProductInventoryReserveRequestedEventHandler>();
builder.Services.AddSingleton<ProductInventoryReleaseRequestedEventHandler>();

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
        var productInventoryRequestedEventHandler = app.Services.GetRequiredService<ProductInventoryReserveRequestedEventHandler>();
        var productInventoryReleasedEventHandler = app.Services.GetRequiredService<ProductInventoryReleaseRequestedEventHandler>();
        eventBus?.Subscribe(productInventoryRequestedEventHandler, nameof(ProductInventoryReserveRequested), "v1");
        eventBus?.Subscribe(productInventoryReleasedEventHandler, nameof(ProductInventoryReleaseRequested), "v1");
    });
});

app.Run();
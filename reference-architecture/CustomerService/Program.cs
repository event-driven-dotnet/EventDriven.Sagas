using Common.Behaviors;
using Common.Integration.Events;
using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Integration.Handlers;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add database settings
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(builder.Configuration);

// Add command and query handlers
builder.Services.AddHandlers(typeof(Program));

// Add behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Add Dapr Event Bus and event handlers
builder.Services.AddDaprEventBus(builder.Configuration);
builder.Services.AddMongoEventCache(builder.Configuration);
builder.Services.AddSingleton<CustomerCreditReserveRequestedEventHandler>();
builder.Services.AddSingleton<CustomerCreditReleaseRequestedEventHandler>();

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
        var customerCreditRequestedEventHandler = app.Services.GetRequiredService<CustomerCreditReserveRequestedEventHandler>();
        var customerCreditReleasedEventHandler = app.Services.GetRequiredService<CustomerCreditReleaseRequestedEventHandler>();
        eventBus?.Subscribe(customerCreditRequestedEventHandler, nameof(CustomerCreditReserveRequested), "v1");
        eventBus?.Subscribe(customerCreditReleasedEventHandler, nameof(CustomerCreditReleaseRequested), "v1");
    });
});

app.Run();
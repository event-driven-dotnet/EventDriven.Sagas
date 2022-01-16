using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Handlers;
using CustomerService.Repositories;
using EventDriven.DependencyInjection.URF.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Command handler registrations
builder.Services.AddSingleton<CustomerCommandHandler>();

// Database registrations
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using SagaSnapshotService.Configuration;
using SagaSnapshotService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrations
builder.Services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(builder.Configuration);
builder.Services.AddSingleton<ISagaSnapshotDtoRepository, SagaSnapshotDtoRepository>();

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
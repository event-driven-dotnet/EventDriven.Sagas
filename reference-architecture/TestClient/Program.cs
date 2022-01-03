using System.Text.Json;
using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestClient.Configuration;
using TestClient.DTO.Write;
using TestClient.Helpers;
using TestClient.Serialization;
using TestClient.Services;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        services.AddAppSettings<SagaConfigServiceSettings>(config);
        services.AddAppSettings<OrderServiceSettings>(config);
        services.AddHttpClient();
        services.AddAutoMapper(typeof(Program));
        services.AddTransient<SagaConfigService>();
        services.AddTransient<OrderService>();
    })
    .Build();

var settings = host.Services.GetRequiredService<SagaConfigServiceSettings>();

Console.WriteLine("Add / update saga configuration? {Y} {N}");
// TODO: Fix auto mapping profile in SagaConfigService
// var key1 = Console.ReadKey().Key;
var key1 = ConsoleKey.N;
if (key1 == ConsoleKey.Y)
{
    // Get saga config
    SagaConfiguration? localSagaConfig = null;
    if (settings.CreateSagaConfig)
    {
        localSagaConfig = CreateSagaConfig(settings.SagaConfigId);
        SaveLocalSagaConfig(localSagaConfig);
    }

    if (localSagaConfig == null)
    {
        localSagaConfig = ReadLocalSagaConfig();
        if (localSagaConfig == null)
        {
            Console.WriteLine("SagaConfig.json not found.");
            return;
        }
    }

    var sagaConfigService = host.Services.GetRequiredService<SagaConfigService>();
    var response = await sagaConfigService.UpsertSagaConfiguration(localSagaConfig);

    // If none add, otherwise update
    if (response == null)
        await sagaConfigService.PostSagaConfiguration(localSagaConfig);
    else
        await sagaConfigService.PutSagaConfiguration(localSagaConfig);
}

// Create an order
Console.WriteLine("\nPress Enter to create an order.");
Console.ReadLine();

var order = new Order
{
    Id = Guid.NewGuid(),
    CustomerId = Guid.NewGuid(),
    OrderDate = DateTime.Now,
    OrderItems = new List<OrderItem>
    {
        new(Guid.NewGuid(), "Espresso", 1M),
        new(Guid.NewGuid(), "Cappuccino", 2M),
        new(Guid.NewGuid(), "Latte", 3M),
    }
};

var orderService = host.Services.GetRequiredService<OrderService>();
var result = await orderService.CreateOrder(order);
Console.WriteLine($"Create Order Saga started. Order state: {result?.State}");

// Monitor status
Console.WriteLine("\nMonitoring status. Press any key to exit.");
while (!Console.KeyAvailable)
{
    await Task.Delay(TimeSpan.FromSeconds(3));
    var orderState = orderService.GetOrderState(settings.SagaConfigId);
    Console.WriteLine($"Order state: {orderState}");
}

void SaveLocalSagaConfig(SagaConfiguration sagaConfig)
{
    var mapper = host.Services.GetRequiredService<IMapper>();
    var serializableSagaConfig = mapper.Map<SerializableSagaConfiguration>(sagaConfig);
    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    var json = JsonSerializer.Serialize(serializableSagaConfig, options);
    File.WriteAllText(settings.SagaConfigPath, json);
}

SagaConfiguration? ReadLocalSagaConfig()
{
    if (settings?.SagaConfigPath == null) return null;
    var json = File.ReadAllText(settings.SagaConfigPath);
    var options = new JsonSerializerOptions
    {
        Converters = { new InterfaceConverterFactory<SetStateCommand, ISagaCommand>() }
    };
    return JsonSerializer.Deserialize<SagaConfiguration>(json, options);
}

SagaConfiguration CreateSagaConfig(Guid id)
{
    var steps = new Dictionary<int, SagaStep>
    {
        {
            1,
            new SagaStep
            {
                Sequence = 1,
                Action = new SagaAction
                {
                    Command = new SetStateCommand
                    {
                        Name = "SetStatePending",
                        ExpectedResult = OrderState.Pending
                    }
                },
                CompensatingAction = new SagaAction
                {
                    Command = new SetStateCommand
                    {
                        Name = "SetStateInitial",
                        ExpectedResult = OrderState.Initial
                    }
                }
            }
        },
        //{   2,
        //    new SagaStep
        //    {
        //        Sequence = 2,
        //        Action = new SagaAction
        //        {
        //            Command = new FakeCommand
        //            {
        //                Name = "ReserveCredit",
        //                Result = "Reserved",
        //                ExpectedResult = "Reserved"
        //            }
        //        },
        //        CompensatingAction = new SagaAction
        //        {
        //            Command = new FakeCommand
        //            {
        //                Name = "ReleaseCredit",
        //                Result = "Available",
        //                ExpectedResult = "Available"
        //            }
        //        }
        //    }
        //},
        //{   3,
        //    new SagaStep
        //    {
        //        Sequence = 3,
        //        Action = new SagaAction
        //        {
        //            Command = new FakeCommand
        //            {
        //                Name = "ReserveInventory",
        //                Result = "Reserved",
        //                ExpectedResult = "Reserved"
        //            }
        //        },
        //        CompensatingAction = new SagaAction
        //        {
        //            Command = new FakeCommand
        //            {
        //                Name = "ReleaseInventory",
        //                Result = "Available",
        //                ExpectedResult = "Available"
        //            }
        //        }
        //    }
        //},
        //{   4,
        //    new SagaStep
        //    {
        //        Sequence = 4,
        //        Action = new SagaAction
        //        {
        //            Command = new SetStateCommand
        //            {
        //                Name = "SetStateCreated",
        //                Result = OrderState.Created,
        //                ExpectedResult = OrderState.Created
        //            }
        //        },
        //        CompensatingAction = new SagaAction
        //        {
        //            Command = new SetStateCommand
        //            {
        //                Name = "SetStateInitial",
        //                Result = OrderState.Initial,
        //                ExpectedResult = OrderState.Initial
        //            }
        //        }
        //    }
        //},
    };
    return new SagaConfiguration { Id = id, Steps = steps, Name = "CreateOrderSaga"};
}
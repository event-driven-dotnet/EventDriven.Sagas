using System.Text.Encodings.Web;
using System.Text.Json;
using EventDriven.DependencyInjection;
using EventDriven.Sagas.Abstractions.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;
using TestClient.Configuration;
using TestClient.DTO;
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
        services.AddTransient<SagaConfigService>();
        services.AddTransient<TestClient.Services.OrderService>();
    })
    .Build();

var settings = host.Services.GetRequiredService<SagaConfigServiceSettings>();

Console.WriteLine("Create saga configuration? {Y} {N}");
SagaConfigurationDto? localSagaConfig = null;
var key1 = settings.Debug ? ConsoleKey.Y : Console.ReadKey(true).Key;
Console.WriteLine(key1);
if (key1 == ConsoleKey.Y)
{
    localSagaConfig = CreateSagaConfig(settings.SagaConfigId);
    SaveLocalSagaConfig(localSagaConfig);
}

Console.WriteLine("\nUpdate saga configuration? {Y} {N}");
var key2 = settings.Debug ? ConsoleKey.N : Console.ReadKey(true).Key;
Console.WriteLine(key2);
if (key2 == ConsoleKey.Y)
{
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
    await sagaConfigService.UpsertSagaConfiguration(localSagaConfig);
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

var orderService = host.Services.GetRequiredService<TestClient.Services.OrderService>();
var result = await orderService.CreateOrder(order);
Console.WriteLine($"Create Order Saga started. Order state: {result?.State}");

// Monitor status
Console.WriteLine("\nPress Enter to monitor status.");
Console.ReadLine();
Console.WriteLine("Monitoring status. Press any key to exit.");
while (!Console.KeyAvailable)
{
    await Task.Delay(TimeSpan.FromSeconds(3));
    var orderState = orderService.GetOrderState(settings.SagaConfigId);
    Console.WriteLine($"Order state: {orderState}");
}

void SaveLocalSagaConfig(SagaConfigurationDto sagaConfig)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    var json = JsonSerializer.Serialize(sagaConfig, options);
    File.WriteAllText(settings.SagaConfigPath, json);
}

SagaConfigurationDto? ReadLocalSagaConfig()
{
    var json = File.ReadAllText(settings.SagaConfigPath);
    return JsonSerializer.Deserialize<SagaConfigurationDto>(json);
}

SagaConfigurationDto CreateSagaConfig(Guid id)
{
    var steps = new List<SagaStepDto>
    {
        new SagaStepDto
        {
            Sequence = 1,
            Action = new SagaActionDto
            {
                Command = JsonSerializer.Serialize(new SetOrderStatePending()
                {
                    Name = typeof(SetOrderStatePending).FullName,
                    ExpectedResult = OrderState.Pending
                })
            },
            CompensatingAction = new SagaActionDto()
            {
                Command = JsonSerializer.Serialize(new SetOrderStateInitial
                {
                    Name = typeof(SetOrderStateInitial).FullName,
                    ExpectedResult = OrderState.Initial
                })
            }
        }
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
    return new SagaConfigurationDto() { Id = id, Steps = steps, Name = "CreateOrderSaga"};
}
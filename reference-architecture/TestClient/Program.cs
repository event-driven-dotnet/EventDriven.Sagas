using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestClient.Configuration;
using TestClient.DTO;
using TestClient.Helpers;
using TestClient.Services;

// Debug toggle
var debug = true;

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
        services.AddTransient<OrderService>();
    })
    .Build();

var settings = host.Services.GetRequiredService<SagaConfigServiceSettings>();

Console.WriteLine("Create saga configuration? {Y} {N}");
SagaConfiguration? localSagaConfig = null;
var key1 = debug ? ConsoleKey.Y : Console.ReadKey().Key;
if (key1 == ConsoleKey.Y)
{
    localSagaConfig = CreateSagaConfig(settings.SagaConfigId);
    SaveLocalSagaConfig(localSagaConfig);
}

Console.WriteLine("\nUpdate saga configuration? {Y} {N}");
var key2 = debug ? ConsoleKey.Y : Console.ReadKey().Key;
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
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    var json = JsonSerializer.Serialize(sagaConfig, options);
    File.WriteAllText(settings.SagaConfigPath, json);
}

SagaConfiguration? ReadLocalSagaConfig()
{
    var json = File.ReadAllText(settings.SagaConfigPath);
    return JsonSerializer.Deserialize<SagaConfiguration>(json);
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
                    Command = JsonSerializer.Serialize(new SetStateCommand
                    {
                        Name = "SetStatePending",
                        ExpectedResult = OrderState.Pending
                    })
                },
                CompensatingAction = new SagaAction
                {
                    Command = JsonSerializer.Serialize(new SetStateCommand
                    {
                        Name = "SetStateInitial",
                        ExpectedResult = OrderState.Initial
                    })
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
using System.Diagnostics;
using BoDi;
using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Repositories;
using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using InventoryService.Configuration;
using InventoryService.Domain.InventoryAggregate;
using InventoryService.Repositories;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Sagas.Specs.Configuration;
using OrderService.Sagas.Specs.Repositories;
using SagaConfigService.Repositories;
using SagaSnapshotService.Repositories;

namespace OrderService.Sagas.Specs.Hooks
{
    [Binding]
    public class Hooks
    {
        private Process? _tyeProcess;
        private readonly IObjectContainer _objectContainer;
        private const string TyeArguments = "run" + " ../../..";

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public async Task RegisterServices()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var config = services.BuildServiceProvider()
                        .GetRequiredService<IConfiguration>();
                    services.AddAppSettings<OrderServiceSpecsSettings>(config);
                    services.AddHttpClient();
                    services.AddSingleton<ISagaConfigDtoRepository, SagaConfigDtoRepository>();
                    services.AddSingleton<ISagaSnapshotDtoRepository, SagaSnapshotDtoRepository>();
                    services.AddSingleton<ICustomerRepository, CustomerRepository>();
                    services.AddSingleton<IInventoryRepository, InventoryRepository>();
                    services.AddSingleton<IOrderRepository, OrderRepository>();
                    services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(config);
                    services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(config);
                    services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(config);
                    services.AddMongoDbSettings<InventoryDatabaseSettings, Inventory>(config);
                    services.AddMongoDbSettings<OrderDatabaseSettings, Order>(config);
                })
                .Build();

            var settings = host.Services.GetRequiredService<OrderServiceSpecsSettings>();
            var sagaConfigRepository = host.Services.GetRequiredService<ISagaConfigDtoRepository>();
            var sagaSnapshotRepository = host.Services.GetRequiredService<ISagaSnapshotDtoRepository>();
            var customerRepository = host.Services.GetRequiredService<ICustomerRepository>();
            var inventoryRepository = host.Services.GetRequiredService<IInventoryRepository>();
            var orderRepository = host.Services.GetRequiredService<IOrderRepository>();
            var httpClient = host.Services.GetRequiredService<HttpClient>();
            httpClient.BaseAddress = new Uri(settings.OrderBaseAddress!);
            
            if (settings.StartTyeProcess)
                await StartTyeProcess(settings.TyeProcessTimeout);

            await ClearData(sagaConfigRepository, settings.SagaConfigId);
            await ClearData(sagaSnapshotRepository, settings.SagaConfigId);
            await ClearData(customerRepository, settings.CustomerId);
            await ClearData(inventoryRepository, settings.InventoryId);
            await ClearData(orderRepository, settings.OrderId);
            
            _objectContainer.RegisterInstanceAs(settings);
            _objectContainer.RegisterInstanceAs(httpClient);
            _objectContainer.RegisterInstanceAs(new JsonFilesRepository());
            _objectContainer.RegisterInstanceAs(sagaConfigRepository);
            _objectContainer.RegisterInstanceAs(customerRepository);
            _objectContainer.RegisterInstanceAs(inventoryRepository);
            _objectContainer.RegisterInstanceAs(orderRepository);
        }

        [AfterScenario]
        public void CleanUp()
        {
            try
            {
                if (_tyeProcess is { HasExited: false })
                    _tyeProcess.Kill();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task ClearData<TRepository>(TRepository repository, Guid entityId)
        {
            if (repository is ISagaConfigDtoRepository sagaConfigRepository)
                await sagaConfigRepository.RemoveAsync(entityId);
            if (repository is ISagaSnapshotDtoRepository sagaSnapshotRepository)
                await sagaSnapshotRepository.RemoveSagaAsync(entityId);
            if (repository is ICustomerRepository customerRepository)
                await customerRepository.RemoveAsync(entityId);
            if (repository is IInventoryRepository inventoryRepository)
                await inventoryRepository.RemoveAsync(entityId);
            if (repository is IOrderRepository orderRepository)
                await orderRepository.RemoveAsync(entityId);
        }

        private async Task StartTyeProcess(TimeSpan waitForTyeProcess)
        {
            var processInfo = new ProcessStartInfo("tye", TyeArguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            _tyeProcess = Process.Start(processInfo);
            await Task.Delay(waitForTyeProcess);
        }
    }
}
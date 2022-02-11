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
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Sagas.Specs.Configuration;
using OrderService.Sagas.Specs.Repositories;
using SagaConfigService.Repositories;

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
                    services.AddSingleton<ICustomerRepository, CustomerRepository>();
                    services.AddSingleton<IOrderRepository, OrderRepository>();
                    services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(config);
                    services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(config);
                    services.AddMongoDbSettings<OrderDatabaseSettings, Order>(config);
                })
                .Build();

            var settings = host.Services.GetRequiredService<OrderServiceSpecsSettings>();
            var sagaConfigRepository = host.Services.GetRequiredService<ISagaConfigDtoRepository>();
            var customerRepository = host.Services.GetRequiredService<ICustomerRepository>();
            var orderRepository = host.Services.GetRequiredService<IOrderRepository>();
            var httpClient = host.Services.GetRequiredService<HttpClient>();
            httpClient.BaseAddress = new Uri(settings.OrderBaseAddress!);
            
            if (settings.StartTyeProcess)
                StartTyeProcess(settings.TyeProcessTimeout);

            await ClearData(sagaConfigRepository, settings.SagaConfigId);
            await ClearData(customerRepository, settings.CustomerId);
            await ClearData(orderRepository, settings.OrderId);
            
            _objectContainer.RegisterInstanceAs(settings);
            _objectContainer.RegisterInstanceAs(httpClient);
            _objectContainer.RegisterInstanceAs(new JsonFilesRepository());
            _objectContainer.RegisterInstanceAs(sagaConfigRepository);
            _objectContainer.RegisterInstanceAs(customerRepository);
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
                await sagaConfigRepository.RemoveSagaConfigurationAsync(entityId);
            if (repository is ICustomerRepository customerRepository)
                await customerRepository.RemoveAsync(entityId);
            if (repository is IOrderRepository orderRepository)
                await orderRepository.RemoveOrderAsync(entityId);
        }

        private void StartTyeProcess(TimeSpan waitForTyeProcess)
        {
            var processInfo = new ProcessStartInfo("tye", TyeArguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            _tyeProcess = Process.Start(processInfo);
            _tyeProcess?.WaitForExit((int)waitForTyeProcess.TotalMilliseconds);
        }
    }
}
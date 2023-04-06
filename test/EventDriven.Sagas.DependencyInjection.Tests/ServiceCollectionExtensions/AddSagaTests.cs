using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Configuration;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Dispatchers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Evaluators;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Handlers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.Repositories;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Dispatchers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Evaluators;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.DependencyInjection.Tests.ServiceCollectionExtensions;

public class AddSagaTests
{
    private readonly Guid _createSagaConfigId = Guid.Parse("237ccb80-02d1-4b65-80e2-bccb32942b9a");
    private readonly Guid _updateSagaConfigId = Guid.Parse("611e6681-e5e3-48ad-9547-7c646870db55");
    
    [Fact]
    public void WhenAddingCreateOrderSaga_ShouldRegisterSagaTypes()
    {
        var services = new ServiceCollection();

        services.AddSaga<CreateOrderSaga, CreateSagaConfigSettings, CreateOrderSagaCommandDispatcher,
            FakeSagaConfigRepository, FakeSagaSnapshotRepository, FakePersistableSagaRepository<CreateOrderSaga>>(_createSagaConfigId,
            false, typeof(AddSagaTests));
        
        var provider = services.BuildServiceProvider();
        var sagaPool = provider.GetService<ISagaPool<CreateOrderSaga>>();
        var sagaConfigSettings = provider.GetService<CreateSagaConfigSettings>();
        var dispatcher = provider.GetService<CreateOrderSagaCommandDispatcher>();
        var handler1 = provider.GetService<CreateOrderCommandHandler>();
        var handler2 = provider.GetService<SetOrderStateCreatedCommandHandler>();
        var evaluator1 = provider.GetService<SetOrderStateResultEvaluator>();
        var evaluator2 = provider.GetService<ReserveCustomerCreditResultEvaluator>();

        sagaPool.Should().NotBeNull();
        sagaConfigSettings?.SagaConfigId.Should().Be(_createSagaConfigId);
        dispatcher.Should().NotBeNull();
        handler1.Should().NotBeNull();
        handler2.Should().NotBeNull();
        evaluator1.Should().NotBeNull();
        evaluator2.Should().NotBeNull();
    }
    
    [Fact]
    public void WhenAddingUpdateOrderSaga_ShouldRegisterSagaTypes()
    {
        var services = new ServiceCollection();

        services.AddSaga<UpdateOrderSaga, UpdateSagaConfigSettings, UpdateOrderSagaCommandDispatcher,
            FakeSagaConfigRepository, FakeSagaSnapshotRepository, FakePersistableSagaRepository<UpdateOrderSaga>>(_updateSagaConfigId,
            false, typeof(AddSagaTests));
        
        var provider = services.BuildServiceProvider();
        var sagaPool = provider.GetService<ISagaPool<UpdateOrderSaga>>();
        var sagaConfigSettings = provider.GetService<UpdateSagaConfigSettings>();
        var dispatcher = provider.GetService<UpdateOrderSagaCommandDispatcher>();
        var handler1 = provider.GetService<UpdateOrderCommandHandler>();
        var handler2 = provider.GetService<SetOrderStatePendingCommandHandler>();
        var evaluator1 = provider.GetService<SetOrderStateResultEvaluator2>();
        var evaluator2 = provider.GetService<ReleaseCustomerCreditResultEvaluator>();

        sagaPool.Should().NotBeNull();
        sagaConfigSettings?.SagaConfigId.Should().Be(_updateSagaConfigId);
        dispatcher.Should().NotBeNull();
        handler1.Should().NotBeNull();
        handler2.Should().NotBeNull();
        evaluator1.Should().NotBeNull();
        evaluator2.Should().NotBeNull();
    }
    
    [Fact]
    public void WhenAddingCreateAndUpdateOrderSaga_ShouldRegisterSagaTypes()
    {
        var services = new ServiceCollection();

        services.AddSaga<CreateOrderSaga, CreateSagaConfigSettings, CreateOrderSagaCommandDispatcher,
            FakeSagaConfigRepository, FakeSagaSnapshotRepository, FakePersistableSagaRepository<CreateOrderSaga>>(_createSagaConfigId,
            false, typeof(AddSagaTests));
        services.AddSaga<UpdateOrderSaga, UpdateSagaConfigSettings, UpdateOrderSagaCommandDispatcher,
            FakeSagaConfigRepository, FakeSagaSnapshotRepository, FakePersistableSagaRepository<UpdateOrderSaga>>(_updateSagaConfigId,
            false, typeof(AddSagaTests));
        
        var provider = services.BuildServiceProvider();
        var createSagaPool = provider.GetService<ISagaPool<CreateOrderSaga>>();
        var createSagaConfigSettings = provider.GetService<CreateSagaConfigSettings>();
        var createDispatcher = provider.GetService<CreateOrderSagaCommandDispatcher>();
        var createHandler1 = provider.GetService<CreateOrderCommandHandler>();
        var createHandler2 = provider.GetService<SetOrderStateCreatedCommandHandler>();
        var createEvaluator1 = provider.GetService<SetOrderStateResultEvaluator>();
        var createEvaluator2 = provider.GetService<ReserveCustomerCreditResultEvaluator>();

        var updateSagaPool = provider.GetService<ISagaPool<UpdateOrderSaga>>();
        var updateSagaConfigSettings = provider.GetService<UpdateSagaConfigSettings>();
        var updateDispatcher = provider.GetService<UpdateOrderSagaCommandDispatcher>();
        var updateHandler1 = provider.GetService<UpdateOrderCommandHandler>();
        var updateHandler2 = provider.GetService<SetOrderStatePendingCommandHandler>();
        var updateEvaluator1 = provider.GetService<SetOrderStateResultEvaluator2>();
        var updateEvaluator2 = provider.GetService<ReleaseCustomerCreditResultEvaluator>();

        createSagaPool.Should().NotBeNull();
        createSagaConfigSettings?.SagaConfigId.Should().Be(_createSagaConfigId);
        createDispatcher.Should().NotBeNull();
        createHandler1.Should().NotBeNull();
        createHandler2.Should().NotBeNull();
        createEvaluator1.Should().NotBeNull();
        createEvaluator2.Should().NotBeNull();
        
        updateSagaPool.Should().NotBeNull();
        updateSagaConfigSettings?.SagaConfigId.Should().Be(_updateSagaConfigId);
        updateDispatcher.Should().NotBeNull();
        updateHandler1.Should().NotBeNull();
        updateHandler2.Should().NotBeNull();
        updateEvaluator1.Should().NotBeNull();
        updateEvaluator2.Should().NotBeNull();
    }
}
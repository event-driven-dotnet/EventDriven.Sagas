using System.Text.Json;
using AutoMapper;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.Mapping;
using EventDriven.Sagas.Persistence.Abstractions.Tests.Fakes;

namespace EventDriven.Sagas.Persistence.Abstractions.Tests;

public class SagaPersistAutoMapperProfileTests
{
    [Fact]
    public void Map_PersistableSagaOfTMetadata_To_PersistableSagaMetadataDto()
    {
        // Arrange
        var mapperConfig = new MapperConfiguration(
            cfg => cfg.AddProfile<SagaPersistAutoMapperProfile>());
        var mapper = mapperConfig.CreateMapper();
        var source = CreateSaga();
        
        // Act
        var destination = mapper.Map<PersistableSagaMetadataDto>(source);
        
        // Assert
        var expectedMetadata = JsonSerializer.Serialize(source.Metadata!);
        Assert.Equal(source.Id, destination.SagaId);
        Assert.Equal(source.Started, destination.SagaStarted);
        Assert.Equal(expectedMetadata, destination.Metadata);
    }
    
    [Fact]
    public void Map_PersistableSagaMetadataDto_To_PersistableSagaOfTMetadata()
    {
        // Arrange
        var mapperConfig = new MapperConfiguration(
            cfg => cfg.AddProfile<SagaPersistAutoMapperProfile>());
        var mapper = mapperConfig.CreateMapper();
        var source = CreateSagaDto();
        var destination = CreateSaga();
        destination.Id = Guid.Empty;
        destination.Metadata = null;
        destination.Started = new DateTime();
        
        // Act
        mapper.Map(source, destination);
        
        // Assert
        Assert.Equal(source.SagaId, destination.Id);
        Assert.Equal(source.SagaStarted, destination.Started);
    }

    private PersistableSagaMetadataDto CreateSagaDto()
    {
        // Arrange
        var mapperConfig = new MapperConfiguration(
            cfg => cfg.AddProfile<SagaPersistAutoMapperProfile>());
        var mapper = mapperConfig.CreateMapper();
        var source = CreateSaga();
        
        // Act
        var destination = mapper.Map<PersistableSagaMetadataDto>(source);
        return destination;
    }
    
    private FakeSaga CreateSaga()
    {
        var dispatcher = new FakeCommandDispatcher();
        var configRepo = new FakeSagaConfigRepository();
        var config = configRepo.GetAsync(Guid.Empty).Result;
        // ReSharper disable once CollectionNeverUpdated.Local
        var resultEvaluators = new List<ISagaCommandResultEvaluator>();
        var factory = new SagaFactory<FakeSaga>(dispatcher, resultEvaluators,
            Enumerable.Empty<ICheckSagaLockCommandHandler>());
        var sagaPool = new InMemorySagaPool<FakeSaga>(factory, new List<ISagaCommandResultDispatcher>(), false);
        var metadata = new FakeMetadata { Age = 29, Name = "John" };
        var saga = new FakeSaga(metadata, config?.Steps!, dispatcher, resultEvaluators, sagaPool)
        {
            Id = Guid.NewGuid()
        };
        return saga;
    }
}
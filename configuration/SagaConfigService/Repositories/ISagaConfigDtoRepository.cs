using EventDriven.Sagas.Abstractions.DTO;

namespace SagaConfigService.Repositories;

public interface ISagaConfigDtoRepository
{
    Task<SagaConfigurationDto?> GetSagaConfigurationAsync(Guid id);

    Task<SagaConfigurationDto?> AddSagaConfigurationAsync(SagaConfigurationDto entity);

    Task<SagaConfigurationDto?> UpdateSagaConfigurationAsync(SagaConfigurationDto entity);

    Task<int> RemoveSagaConfigurationAsync(Guid id);
}
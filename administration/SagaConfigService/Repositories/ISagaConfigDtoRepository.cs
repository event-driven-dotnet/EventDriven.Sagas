using EventDriven.Sagas.Configuration.Abstractions.DTO;

namespace SagaConfigService.Repositories;

public interface ISagaConfigDtoRepository
{
    Task<SagaConfigurationDto?> GetAsync(Guid id);

    Task<SagaConfigurationDto?> AddAsync(SagaConfigurationDto entity);

    Task<SagaConfigurationDto?> UpdateAsync(SagaConfigurationDto entity);

    Task<int> RemoveAsync(Guid id);
}
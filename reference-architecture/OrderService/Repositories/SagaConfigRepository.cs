using AutoMapper;
using EventDriven.Sagas.Abstractions.DTO;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Repositories;
using URF.Core.Abstractions;

namespace OrderService.Repositories;

public class SagaConfigRepository : ISagaConfigRepository
{
    private readonly IMapper _mapper;
    private readonly IDocumentRepository<SagaConfigurationDto> _documentRepository;

    public SagaConfigRepository(
        IDocumentRepository<SagaConfigurationDto> documentRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<SagaConfiguration?> GetSagaConfigurationAsync(Guid id)
    {
        var dto = await _documentRepository.FindOneAsync(e => e.Id == id);
        return _mapper.Map<SagaConfiguration>(dto);
    }

    public async Task<SagaConfiguration> AddSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existingDto = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
        if (existingDto != null) throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await _documentRepository.InsertOneAsync(dto);
        return _mapper.Map<SagaConfiguration>(dto);
    }

    public async Task<SagaConfiguration> UpdateSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existingDto = await GetSagaConfigurationAsync(entity.Id);
        if (existingDto == null || string.Compare(entity.ETag, existingDto.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, dto);
        return _mapper.Map<SagaConfiguration>(dto);
    }

    public async Task<int> RemoveSagaConfigurationAsync(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}
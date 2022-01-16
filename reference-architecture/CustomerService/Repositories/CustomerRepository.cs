using CustomerService.Domain.CustomerAggregate;
using EventDriven.DDD.Abstractions.Repositories;
using URF.Core.Abstractions;

namespace CustomerService.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDocumentRepository<Customer> _documentRepository;

    public CustomerRepository(
        IDocumentRepository<Customer> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IEnumerable<Customer>> Get() =>
        await _documentRepository.FindManyAsync();

    public async Task<Customer?> Get(Guid id) =>
        await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<Customer?> Add(Customer entity)
    {
        var existing = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
        if (existing != null) return null;
        entity.SequenceNumber = 1;
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.InsertOneAsync(entity);
    }

    public async Task<Customer?> Update(Customer entity)
    {
        var existing = await Get(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
            throw new ConcurrencyException();
        entity.SequenceNumber = existing.SequenceNumber + 1;
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> Remove(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}
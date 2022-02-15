using AutoMapper;
using InventoryService.DTO;
using InventoryService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[Route("api/inventory")]
[ApiController]
public class InventoryQueryController : ControllerBase
{
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;

    public InventoryQueryController(
        IInventoryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
        
    // GET: api/inventory
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var inventories = await _repository.GetAsync();
        var result = _mapper.Map<IEnumerable<Inventory>>(inventories);
        return Ok(result);
    }

    // GET: api/inventory/id
    [HttpGet("{id:guid}", Name = nameof(Get))]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var inventory = await _repository.GetAsync(id);
        if (inventory == null) return NotFound();
        var result = _mapper.Map<Inventory>(inventory);
        return Ok(result);
    }
}
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Extensions;
using InventoryService.Domain.InventoryAggregate;
using InventoryService.Domain.InventoryAggregate.Commands;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryCommandController : ControllerBase
    {
        private readonly ICommandBroker _commandBroker;
        private readonly IMapper _mapper;

        public InventoryCommandController(
            ICommandBroker commandBroker,
            IMapper mapper)
        {
            _commandBroker = commandBroker;
            _mapper = mapper;
        }
        
        // POST: api/inventory
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DTO.Inventory inventoryDto)
        {
            var inventoryIn = _mapper.Map<Inventory>(inventoryDto);
            var result = await _commandBroker.SendAsync(new CreateInventory(inventoryIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var inventoryOut = _mapper.Map<DTO.Inventory>(result.Entity);
            return new CreatedResult($"api/inventory/{inventoryOut.Id}", inventoryOut);
        }

        // PUT: api/inventory/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DTO.Inventory inventoryDto)
        {
            var inventoryIn = _mapper.Map<Inventory>(inventoryDto);
            var result = await _commandBroker.SendAsync(new UpdateInventory(inventoryIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var inventoryOut = _mapper.Map<DTO.Inventory>(result.Entity);
            return result.ToActionResult(inventoryOut);
        }

        // DELETE: api/inventory/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _commandBroker.SendAsync(new RemoveInventory(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult() 
                : new NoContentResult();
        }    }
}
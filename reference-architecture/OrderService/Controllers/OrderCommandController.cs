using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Extensions;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Commands;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderCommandController : ControllerBase
    {
        private readonly ICommandBroker _commandBroker;
        private readonly IMapper _mapper;

        public OrderCommandController(
            ICommandBroker commandBroker,
            IMapper mapper)
        {
            _commandBroker = commandBroker;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Order orderDto)
        {
            var orderIn = _mapper.Map<Domain.OrderAggregate.Order>(orderDto);
            var result = await _commandBroker.SendAsync(new StartCreateOrderSaga(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Order>(result.Entity);
            return Ok(orderOut);
        }
    }
}
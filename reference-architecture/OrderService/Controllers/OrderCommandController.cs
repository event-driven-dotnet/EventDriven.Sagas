using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Commands.Handlers;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderCommandController : ControllerBase
    {
        private readonly StartCreateOrderSagaCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public OrderCommandController(
            StartCreateOrderSagaCommandHandler commandHandler,
            IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Order orderDto)
        {
            var orderIn = _mapper.Map<Domain.OrderAggregate.Order>(orderDto);
            var result = await _commandHandler.Handle(new StartCreateOrderSaga(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Order>(result.Entity);
            return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
        }
    }
}
using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Commands.Handlers;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;
using OrderService.Helpers;
using Order = OrderService.DTO.Order;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderCommandController : ControllerBase
    {
        private readonly CreateOrderCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public OrderCommandController(CreateOrderCommandHandler commandHandler, IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order orderDto)
        {
            var orderIn = _mapper.Map<Domain.OrderAggregate.Order>(orderDto);
            var result = await _commandHandler.Handle(new CreateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<Order>(result.Entity);
            return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
        }
    }
}
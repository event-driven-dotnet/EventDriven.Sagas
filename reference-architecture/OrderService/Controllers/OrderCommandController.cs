using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderCommandController : ControllerBase
    {
        private readonly OrderCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public OrderCommandController(OrderCommandHandler commandHandler, IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Write.Order orderDto)
        {
            var orderIn = _mapper.Map<Order>(orderDto);
            var result = await _commandHandler.Handle(new CreateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
            return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
        }
    }
}
using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrderController : ControllerBase
    {
        private readonly OrderCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public OrderController(OrderCommandHandler commandHandler, IMapper mapper)
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

        // Get api/order/status
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var result = await _commandHandler.Handle(new GetOrderState(orderId));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
            return new OkObjectResult(orderOut);
        }
    }
}
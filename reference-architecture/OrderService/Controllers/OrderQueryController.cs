using AutoMapper;
using EventDriven.CQRS.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Queries;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderQueryController : ControllerBase
    {
        private readonly IQueryBroker _queryBroker;
        private readonly IMapper _mapper;

        public OrderQueryController(
            IQueryBroker queryBroker,
            IMapper mapper)
        {
            _queryBroker = queryBroker;
            _mapper = mapper;
        }

        // GET api/order/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _queryBroker.SendAsync(new GetOrder(id));
            if (order == null) return NotFound();
            var orderOut = _mapper.Map<DTO.Order>(order);
            return Ok(orderOut);
        }

        // Get api/order/state/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet]
        [Route("state/{id:guid}")]
        public async Task<IActionResult> GetOrderState(Guid id)
        {
            var orderState = await _queryBroker.SendAsync(new GetOrderState(id));
            if (orderState == null) return NotFound();
            return Ok(orderState);
        }
    }
}
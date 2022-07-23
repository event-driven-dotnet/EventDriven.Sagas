using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Extensions;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
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
            var orderIn = _mapper.Map<Order>(orderDto);
            var orderMetadata = new OrderMetadata
            {
                VendorInfo = new VendorInfo
                {
                    Name = "Amazon",
                    City = "Seattle",
                    State = "Washington",
                    Country = "USA"
                }
            };
            var result = await _commandBroker.SendAsync(new StartCreateOrderSaga(orderIn, orderMetadata));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Order>(result.Entity);
            return Ok(orderOut);
        }
    }
}
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderQueryController : ControllerBase
    {
        private readonly OrderCommandHandler _commandHandler;

        public OrderQueryController(OrderCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        // Get api/order/status
        [HttpGet]
        [Route("state")]
        public async Task<IActionResult> GetOrderState(Guid orderId)
        {
            var result = await _commandHandler.Handle(new GetOrderState(orderId));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            return new OkObjectResult(result.Entity?.State);
        }
    }
}
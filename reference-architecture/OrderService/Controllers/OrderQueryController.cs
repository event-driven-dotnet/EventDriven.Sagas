using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Repositories;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderQueryController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderQueryController(
            IOrderRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET api/order/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _repository.GetAsync(id);
            if (result == null) return NotFound();
            var orderOut = _mapper.Map<DTO.Order>(result);
            return Ok(orderOut);
        }

        // Get api/order/status/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet]
        [Route("state/{id:guid}")]
        public async Task<IActionResult> GetOrderState(Guid id)
        {
            var orderState = await _repository.GetOrderStateAsync(id);
            if (orderState == null) return NotFound();
            return Ok(orderState);
        }
    }
}
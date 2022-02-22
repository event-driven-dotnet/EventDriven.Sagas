using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using Microsoft.AspNetCore.Mvc;
using SagaConfigService.Repositories;

namespace SagaConfigService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaConfigController : ControllerBase
    {
        private readonly ISagaConfigDtoRepository _configRepository;
        private readonly ILogger<SagaConfigController> _logger;

        public SagaConfigController(
            ISagaConfigDtoRepository configRepository,
            ILogger<SagaConfigController> logger)
        {
            _configRepository = configRepository;
            _logger = logger;
        }

        // GET api/sagaconfig/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _configRepository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/sagaconfig
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SagaConfigurationDto value)
        {
            try
            {
                var result = await _configRepository.AddAsync(value);
                return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
            }
            catch (ConcurrencyException e)
            {
                _logger.LogError(e, "{Message}", e.Message);
                return Conflict();
            }
        }

        // PUT api/sagaconfig
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SagaConfigurationDto value)
        {
            try
            {
                var result = await _configRepository.UpdateAsync(value);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ConcurrencyException e)
            {
                _logger.LogError(e, "{Message}", e.Message);
                return Conflict();
            }
        }

        // DELETE api/<sagaconfig/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result =await _configRepository.RemoveAsync(id);
            return Ok(result);
        }
    }
}

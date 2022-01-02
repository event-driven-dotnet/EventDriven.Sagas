using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SagaConfigService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaConfigController : ControllerBase
    {
        private readonly ISagaConfigRepository _configRepository;

        public SagaConfigController(ISagaConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        // GET api/<SagaConfigController>/
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _configRepository.GetSagaConfigurationAsync(id);
            return Ok(result);
        }

        // POST api/<SagaConfigController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SagaConfiguration value)
        {
            var result = await _configRepository.AddSagaConfigurationAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
        }

        // PUT api/<SagaConfigController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SagaConfiguration value)
        {
            var result = await _configRepository.UpdateSagaConfigurationAsync(value);
            return Ok(result);
        }

        // DELETE api/<SagaConfigController>/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _configRepository.RemoveSagaConfigurationAsync(id);
            return NoContent();
        }
    }
}

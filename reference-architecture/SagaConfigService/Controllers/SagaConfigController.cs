using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SagaConfigService.Repositories;
using DTO = SagaConfigService.DTO;
using Entities = SagaConfigService.Entities;

namespace SagaConfigService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaConfigController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISagaConfigRepository _configRepository;

        public SagaConfigController(IMapper mapper, ISagaConfigRepository configRepository)
        {
            _mapper = mapper;
            _configRepository = configRepository;
        }

        // GET api/<SagaConfigController>/
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _configRepository.GetSagaConfigurationAsync(id);
            // TODO: Fix conversion of Command from BsonDocument to Json
            var sagaConfigOut = _mapper.Map<DTO.SagaConfiguration>(result);
            return Ok(sagaConfigOut);
        }

        // POST api/<SagaConfigController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DTO.SagaConfiguration value)
        {
            var sagaConfigIn = _mapper.Map<Entities.SagaConfiguration>(value);
            try
            {
                var result = await _configRepository.AddSagaConfigurationAsync(sagaConfigIn);
                // TODO: Fix conversion of Command from BsonDocument to Json
                // var sagaConfigOut = _mapper.Map<DTO.SagaConfiguration>(result);
                value.ETag = result.ETag;
                return CreatedAtAction(nameof(Get), new { id = value.Id }, value); // sagaConfigOut
            }
            catch (ConcurrencyException e)
            {
                Console.WriteLine(e);
                return Conflict();
            }
        }

        // PUT api/<SagaConfigController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DTO.SagaConfiguration value)
        {
            var sagaConfigIn = _mapper.Map<Entities.SagaConfiguration>(value);
            try
            {
                var result = await _configRepository.UpdateSagaConfigurationAsync(sagaConfigIn);
                // TODO: Fix conversion of Command from BsonDocument to Json
                // var sagaConfigOut = _mapper.Map<DTO.SagaConfiguration>(result);
                value.ETag = result.ETag;
                return Ok(value); // sagaConfigOut
            }
            catch (ConcurrencyException e)
            {
                Console.WriteLine(e);
                return Conflict();
            }
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

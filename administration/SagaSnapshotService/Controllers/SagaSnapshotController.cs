using Microsoft.AspNetCore.Mvc;
using SagaSnapshotService.Repositories;

namespace SagaSnapshotService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaSnapshotController : ControllerBase
    {
        private readonly ISagaSnapshotDtoRepository _snapshotRepository;

        public SagaSnapshotController(
            ISagaSnapshotDtoRepository snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }

        // GET api/sagasnapshot/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSnapshot(Guid id)
        {
            var result = await _snapshotRepository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET api/sagasnapshot/sagas/d89ffb1e-7481-4111-a4dd-ac5123217293
        [HttpGet("sagas/{sagaId:guid}")]
        public async Task<IActionResult> GetSagaSnapshots(Guid sagaId)
        {
            var result = await _snapshotRepository.GetSagaAsync(sagaId);
            return Ok(result);
        }
    }
}

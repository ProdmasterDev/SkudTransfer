using Microsoft.AspNetCore.Mvc;
using SkudTransfer.Transfers;
using SkudTransferApi.Transfers;

namespace SkudTransferApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ControllerTransfer _controllerTransfer;
        private readonly WorkerGroupTransfer _workerGroupTransfer;
        private readonly WorkerTransfer _workerTransfer;
        private readonly CardTransfer _cardTransfer;
        private readonly EventTransfer _eventTransfer;
        private readonly AccessTransfer _accessTransfer;
        private readonly AccessGroupTransfer _accessGroupTransfer;
        private readonly WorkerGroupAccessTransfer _workerGroupAccessTransfer;
        private readonly WorkerAccessGroupTransfer _workerAccessGroupTransfer;
        private readonly RefreshQuickAccessTransfer _refreshQuickAccessTransfer;
        public TransferController(ControllerTransfer controllerTransfer, WorkerGroupTransfer workerGroupTransfer, WorkerTransfer workerTransfer, CardTransfer cardTransfer, EventTransfer eventTransfer, AccessTransfer accessTransfer, AccessGroupTransfer accessGroupTransfer, WorkerGroupAccessTransfer workerGroupAccessTransfer, WorkerAccessGroupTransfer workerAccessGroupTransfer, RefreshQuickAccessTransfer refreshQuickAccessTransfer)
        {
            _controllerTransfer = controllerTransfer;
            _workerGroupTransfer = workerGroupTransfer;
            _workerTransfer = workerTransfer;
            _cardTransfer = cardTransfer;
            _eventTransfer = eventTransfer;
            _accessTransfer = accessTransfer;
            _accessGroupTransfer = accessGroupTransfer;
            _workerGroupAccessTransfer = workerGroupAccessTransfer;
            _workerAccessGroupTransfer = workerAccessGroupTransfer;
            _workerAccessGroupTransfer = workerAccessGroupTransfer;
            _refreshQuickAccessTransfer = refreshQuickAccessTransfer;
        }
        [HttpGet("Controllers")]
        public async Task<IActionResult> Controllers()
        {
            await _controllerTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("WorkerGroups")]
        public async Task<IActionResult> WorkerGroups()
        {
            await _workerGroupTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("Workers")]
        public async Task<IActionResult> Workers()
        {
            await _workerTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("Cards")]
        public async Task<IActionResult> Cards()
        {
            await _cardTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("Events")]
        public async Task<IActionResult> Events()
        {
            await _eventTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("Accesses")]
        public async Task<IActionResult> Accesses()
        {
            await _accessTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("AccessGroup")]
        public async Task<IActionResult> AccessGroup()
        {
            await _accessGroupTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("WorkerGroupAccess")]
        public async Task<IActionResult> WorkerGroupAccess()
        {
            await _workerGroupAccessTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("WorkerAccessGroup")]
        public async Task<IActionResult> WorkerAccessGroup()
        {
            await _workerAccessGroupTransfer.DoTransfer();
            return Ok();
        }
        [HttpGet("RefreshQuickAccess")]
        public async Task<IActionResult> RefreshQuickAccess()
        {
            await _refreshQuickAccessTransfer.DoTransfer();
            return Ok();
        }
    }
}

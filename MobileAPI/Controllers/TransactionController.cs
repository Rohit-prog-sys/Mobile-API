using Microsoft.AspNetCore.Mvc;
using MobileAPI.DTOs;
using MobileAPI.DTOs.Common;
using MobileAPI.Interface;

namespace MobileAPI.Controllers
{


    [ApiController]
    [Route("api/txn")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repo;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(ITransactionRepository repo, ILogger <TransactionController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateStatus([FromBody] TxnUpdateDto dto)
        {
            
            await _repo.UpdateStatusAsync(dto.TxnId, dto.Status, dto.ResponseCode);
            _logger.LogInformation("Transaction status updated. TxnId={TxnId}", dto.TxnId);
            return Ok();
        }

        [HttpGet("{txnId}")]
        public async Task<IActionResult> GetStatus(string txnId)
        {
            _logger.LogInformation("Transaction check status. TxnId={TxnId}", txnId);

            var txn = await _repo.GetAsync(txnId);

            if (txn == null)
                return NotFound();

            return Ok(new
            {
                txn.TxnId,
                txn.Status,
                txn.ResponseCode
            });
       
        }
    }

}

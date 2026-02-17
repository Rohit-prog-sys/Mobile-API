using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.DTOs;
using MobileAPI.DTOs.Common;
using MobileAPI.Interface;
using MobileAPI.Models;

namespace MobileAPI.Controllers
{

    //[Authorize]
    [ApiController]
    [Route("api/pay")]
    public class PayController : ControllerBase
    {
        private readonly ISwitchService _switchService;
        private readonly ITransactionRepository _txnRepo;
        private readonly ILogger<PayController> _logger;
        private readonly IAccountService _accountService;


        public PayController(ISwitchService switchService,ITransactionRepository txnRepo, IAccountService accountService, ILogger<PayController> logger)
        {
            _switchService = switchService;
            _txnRepo = txnRepo;
            _accountService = accountService;
            _logger = logger;
        }

        //[HttpPost]
        //public async Task<IActionResult> Pay([FromBody] PayRequestDto request)
        //{
        //    var txnId = Guid.NewGuid().ToString("N");

        //    try
        //    {
        //        _logger.LogInformation("Pay request received {@Request}", request);

        //        if (request.Amount <= 0)
        //        {
        //            _logger.LogWarning("Invalid amount {Amount}", request.Amount);
        //            return BadRequest("Invalid amount");
        //        }

        //        await _txnRepo.AddAsync(new TransactionEntity
        //        {
        //            TxnId = txnId,
        //            PayerVpa = request.PayerVpa,
        //            PayeeVpa = request.PayeeVpa,
        //            Amount = request.Amount,
        //            Status = "INITIATED"
        //        });

        //        _logger.LogInformation("Transaction saved. TxnId={TxnId}", txnId);

        //        var account = await _accountService.GetDetailsAsync(request.PayerAcNo);
        //      if (account == null)
        //            return BadRequest("Account not found");



        //        var switchRequest = new
        //        {
        //            PayerAddr = request.PayerVpa,
        //            PayeeAddr = request.PayeeVpa,
        //            Amount = request.Amount.ToString("0.00"),
        //            Note = request.Note,
        //            TxnType = request.TxnType,

        //            PayerName= account.CustomerName,
        //            PayerType = account.AcType,
        //            IFSC=account.IFSC,
        //            AccountNumber=account.AccountNumber,
        //            //CustRef = account.CustomerNo,
        //            Device = request.Device,
        //            TxnId = txnId
        //        };

        //        _ = _switchService.SendAsync<object>("ReqPayAcq", "2.0", txnId, switchRequest);

               
        //        _logger.LogInformation("Switch request sent. TxnId={TxnId}", txnId);


        //        await _txnRepo.AddAsync(new TransactionEntity
        //        {
        //            TxnId = txnId,
        //            PayerVpa = request.PayerVpa,
        //            PayeeVpa = request.PayeeVpa,
        //            Amount = request.Amount,
        //            Status = "PENDING"
        //        });


        //        return Ok(new
        //        {
        //            TxnId = txnId,
        //            Status = "PENDING"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Payment failed for TxnId={TxnId}", txnId);

        //        return StatusCode(500, new
        //        {
        //            TxnId = txnId,
        //            Status = "FAILED",
        //            Message = "Internal server error"
        //        });
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PayRequestDto request)
        {
            var txnId = TxnIdGenerator.Generate();

            try
            {
                // 1️⃣ Save INITIATED
                await _txnRepo.AddAsync(new TransactionEntity
                {
                    TxnId = txnId,
                    PayerVpa = request.PayerVpa,
                    PayeeVpa = request.PayeeVpa,
                    Amount = request.Amount,
                    Status = "INITIATED"
                });

                var account = await _accountService.GetDetailsAsync(request.PayerAcNo);
                if (account == null)
                    return BadRequest("Account not found");

                var switchRequest = new
                {
                    PayerAddr = request.PayerVpa,
                    PayeeAddr = request.PayeeVpa,
                    Amount = request.Amount.ToString("0.00"),
                    IFSC = account.IFSC,
                    AccountNumber = account.AccountNumber,
                    TxnId = txnId,
                    EntryMode = request.EntryMode
                };

                // 2️⃣ WAIT for switch response
                var switchResponse =await _switchService.SendAsync<SwitchAckResponseDto>(
                        "ReqPayAcq", "2.0", txnId, switchRequest);

                // 3️⃣ Update based on NPCI
                if (switchResponse.ResponseCode == "00")
                {
                    await _txnRepo.UpdateStatusAsync(txnId, "PENDING", "00");
                }
                else
                {
                    await _txnRepo.UpdateStatusAsync(txnId, "FAILURE", switchResponse.ResponseCode);
                }

                return Ok(new
                {
                    TxnId = txnId,
                    Status = switchResponse.ResponseStatus
                });
            }
            catch (Exception ex)
            {
                await _txnRepo.UpdateStatusAsync(txnId, "FAILURE", "XE");

                _logger.LogError(ex, "Payment failed");

                return StatusCode(500, "Switch failure");
            }
        }

        public static class TxnIdGenerator
        {
            public static string Generate()
            {
                return "MIS" + Guid.NewGuid().ToString("N")[..29];
            }
        }

    }


}

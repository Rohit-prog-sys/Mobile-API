using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.DTOs;
using MobileAPI.Interface;

namespace MobileAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/mandate")]
    public class MandateController : ControllerBase
    {
        private readonly ISwitchService _switchService;

        public MandateController(ISwitchService switchService)
        {
            _switchService = switchService;
        }

        [HttpPost]
        public async Task<IActionResult> Mandate([FromBody] MandateRequestDto request)
        {
            if (request.Amount <= 0)
                return BadRequest("Invalid amount");

            var switchRequest = new
            {
                PayerAddr = request.PayerVpa,
                PayeeAddr = request.PayeeVpa,
                Amount = request.Amount.ToString("0.00"),
                Note = request.Note,
                TxnType = request.TxnType,
                TxnId = request.TxnID
            };

            var response = await _switchService.SendAsync<MandateResponseDto>("ReqMandateAcq", "2.0", switchRequest.TxnId, switchRequest);
            return Ok(response);
        }



    }


}

using System.Net;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.DTOs;
using MobileAPI.DTOs.Common;
using MobileAPI.Interface;

namespace MobileAPI.Controllers
{

    [ApiController]
    [Route("api/otp")]
    public class UserRegistrationController : ControllerBase
    {
        private readonly OTPService _service;
        private readonly ILogger<UserRegistrationController> _logger;
        private readonly IAccountService _accountService;

        public UserRegistrationController(OTPService service, ILogger<UserRegistrationController> logger, IAccountService accountService)
        {
            _service = service;
            _logger = logger;
            _accountService = accountService;
        }

        [HttpPost("getotp")]
        public async Task<IActionResult> GetOTP([FromBody] OTPDetailsRequestDto request)
        {
            //var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _service.GenerateAsync(request.MobileNo, request.DeviceID);

            return Ok(new OTPDetailsResponseDto
            {
                ResponseMessage = "OTP Sent Successfully",
                ResponseCode = "00"
            });

        }


        [HttpPost("verify")]
        public async Task<IActionResult> Verify(OTPDetailsRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MobileNo) ||
                    string.IsNullOrWhiteSpace(request.Otp))
                {
                    return Ok(new AccountListResponseDto
                    {
                        ResponseCode = "01",
                        ResponseMessage = "Mobile number and OTP are required"
                    });
                }

                var result = await _service.VerifyOtpAsync(request.MobileNo, request.Otp);

                if (!result.Success)
                {
                    return Ok(new AccountListResponseDto
                    {
                        ResponseCode = "01",
                        ResponseMessage = result.Message
                    });
                }

                var accountList =
                    await _accountService.GetAccountListAsync(request.MobileNo);

                if (accountList == null || accountList.Accounts == null)
                {
                    return Ok(new AccountListResponseDto
                    {
                        ResponseCode = "01",
                        ResponseMessage = "No accounts found"
                    });
                }

                return Ok(new AccountListResponseDto
                {
                    ResponseCode = "00",
                    ResponseMessage = "OTP verified successfully",
                    Accounts = accountList.Accounts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTP verification failed");

                return StatusCode(500, new AccountListResponseDto
                {
                    ResponseCode = "01",
                    ResponseMessage = "Internal server error"
                });
            }
        }

    }




}

using Microsoft.AspNetCore.Mvc;
using MobileAPI.DTOs;
using MobileAPI.Interface;

namespace MobileAPI.Controllers
{
    

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            
            if (request.Mobile != "9999999999" || request.Password != "1234")
                return Unauthorized("Invalid credentials");

            var token = _tokenService.GenerateToken("USER001", request.Mobile);

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }



}

using Microsoft.AspNetCore.Mvc;
using YSMConcept.Application.DTOs.AuthDTOs;
using YSMConcept.Application.Services.Interfaces;

namespace YSMConcept.API.Controllers
{
    [Route("login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            var (status, message) = _authService.Login(loginDto);

            if (status == 0)
            {
                _logger.LogWarning(message);
                return BadRequest(message);
            }

            return Ok(message);
        }
    }
}

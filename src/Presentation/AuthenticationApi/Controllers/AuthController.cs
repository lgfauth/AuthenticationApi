using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesApplication.Interfaces;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Opcional: endpoint para validar o token
        [HttpGet("validate")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            var response = _authService.ValidateToken(token);

            if (response is null || !response.IsSuccess)
                return Ok(new { isValid = false });
            
            return Ok(new { isValid = response.Data });
        }
    }
}
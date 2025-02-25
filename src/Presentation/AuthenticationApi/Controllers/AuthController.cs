using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesApplication.Interfaces;
using ServicesApplication.Validators;

namespace AuthenticationApi.Controllers
{
    /// <summary>
    /// Anthenticantion controller 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Anthenticantion controller constructor.
        /// </summary>
        /// <param name="authService">Injectionr of service IAuthService.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Generate token to authenticate other services.
        /// </summary>
        /// <param name="request">LoginRequest object for register.</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                Validations.Validate(request);

                var response = await _authService.LoginAsync(request);
                return Ok(response.Data);
            }
            catch (ValidationException vex)
            {
                return BadRequest(vex.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "LA654" });
            }
        }

        /// <summary>
        /// Verify if token already valid for use.
        /// </summary>
        /// <param name="token">A string of token.</param>
        /// <returns></returns>
        [HttpGet("validate")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            if(string.IsNullOrEmpty(token))
                return BadRequest(new ResponseModel { Message = "Token paramenter can't be null or empty", Code = "LA614" });

            var response = _authService.ValidateToken(token);

            if (response is null || !response.IsSuccess)
                return Ok(new { isValid = false });
            
            return Ok(new ResponseModel { Message = response.Data ? "Valid" : "Not valid", Code = "LA652" });
        }
    }
}
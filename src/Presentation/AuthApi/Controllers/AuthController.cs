using Application.Interfaces;
using Application.LogModels;
using Application.Utils;
using Domain.Models;
using Domain.Validation;
using MicroservicesLogger.Enums;
using MicroservicesLogger.Interfaces;
using MicroservicesLogger.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthApi.Controllers
{
    /// <summary>
    /// Anthenticantion controller 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IApiLog<ApiLogModel> _logger;

        /// <summary>
        /// Anthenticantion controller constructor.
        /// </summary>
        /// <param name="authService">Injectionr of service IAuthService.</param>
        public AuthController(IAuthService authService, IApiLog<ApiLogModel> logger)
        {
            _logger = logger;
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
            var baselog = await _logger.CreateBaseLogAsync();
            var log = new SubLog();

            try
            {
                baselog.Request = Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(request));

                var validation = Validations.Validate(request);
                if (!validation.IsSuccess)
                {
                    baselog.Level = LogTypes.WARN;
                    baselog.Response = new { response = validation!.Error };

                    return BadRequest(validation!.Error);
                }

                var response = await _authService.LoginAsync(request);

                baselog.Level = LogTypes.INFO;
                baselog.Response = Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(response.Data));

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                baselog.Level = LogTypes.ERROR;
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "LA654" });
            }
            finally
            {
                await baselog.AddStepAsync("LOGIN_REQUEST", log);
                await _logger.WriteLogAsync(baselog);
            }
        }

        /// <summary>
        /// Verify if token already valid for use.
        /// </summary>
        /// <param name="token">A string of token.</param>
        /// <returns></returns>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken([FromQuery] string token)
        {
            var baselog = await _logger.CreateBaseLogAsync();
            var log = new SubLog();

            baselog.Request = string.Format("Token is {0}", string.IsNullOrWhiteSpace(token) ? "valid a string" : "invalid a string");

            if (string.IsNullOrEmpty(token))
                return BadRequest(new ResponseModel { Message = "Token paramenter can't be null or empty", Code = "LA614" });

            var response = await _authService.ValidateToken(token);
            
            baselog.Response = response.Data;

            await baselog.AddStepAsync("LOGIN_REQUEST", log);
            await _logger.WriteLogAsync(baselog);

            if (response is null || !response.IsSuccess)
                return Ok(new { isValid = false });

            return Ok(new ResponseModel { Message = response.Data ? "Valid" : "Not valid", Code = "LA652" });
        }
    }
}
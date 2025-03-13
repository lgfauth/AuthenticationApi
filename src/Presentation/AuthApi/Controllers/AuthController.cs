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
                baselog.Endpoint = "POST: Login";

                var validation = Validations.Validate(request);
                if (!validation.IsSuccess)
                {
                    baselog.Level = LogTypes.WARN;
                    baselog.Response = new { response = validation!.Error };

                    return BadRequest(validation!.Error);
                }

                var response = await _authService.LoginAsync(request);
                if (!response.IsSuccess)
                {
                    baselog.Level = LogTypes.WARN;
                    baselog.Response = new { response = response!.Error };

                    return BadRequest(response!.Error);
                }

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
    }
}
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
    /// Registration controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IApiLog<ApiLogModel> _logger;
        private readonly IRegisterService _registerService;

        /// <summary>
        /// Registration controller constructor.
        /// </summary>
        /// <param name="registerService"></param>
        public RegisterController(IRegisterService registerService, IApiLog<ApiLogModel> logger)
        {
            _logger = logger;
            _registerService = registerService;
        }

        /// <summary>
        /// Destinated to receive a registration request.
        /// </summary>
        /// <param name="request">Object RegisterRequest for subscription.</param>
        /// <returns></returns>
        [HttpPost("subscribe")]
        [ProducesResponseType(typeof(ResponseModel), 201)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Register([FromBody] SubscriptionRequest request)
        {
            var baselog = await _logger.CreateBaseLogAsync();
            var log = new SubLog();

            try
            {
                baselog.Request = Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(request));

                Validations.Validate(request);

                var response = await _registerService.RegisterAsync(request);

                var responseModel = new ResponseModel
                {
                    Message = "All done here, see your email to confirm your subscription.",
                    Code = "SC201"
                };

                baselog.Level = LogTypes.INFO;
                baselog.Response = new
                {
                    responseFromService =
                    Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(response.Data)),
                    responseModel
                };

                return StatusCode(201, responseModel);
            }
            catch (ValidationException vex)
            {
                baselog.Level = LogTypes.WARN;
                return BadRequest(vex.Data);
            }
            catch (Exception ex)
            {
                baselog.Level = LogTypes.ERROR;
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "EX058" });
            }
            finally
            {
                await baselog.AddStepAsync("SUBSCRIPTION_REQUEST", log);
                await _logger.WriteLogAsync(baselog);
            }
        }

        /// <summary>
        /// Destinated to receive a register exclusion request.
        /// </summary>
        /// <param name="request">Object UnsubscribeRequest for unsubscription.</param>
        /// <returns></returns>
        [HttpDelete("unsubscribe")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Unregister([FromBody] UnsubscribeRequest request)
        {
            var baselog = await _logger.CreateBaseLogAsync();
            var log = new SubLog();

            try
            {
                baselog.Request = Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(request));

                Validations.Validate(request);

                var response = await _registerService.UnregisterAsync(request);
                if (!response.IsSuccess)
                {
                    baselog.Level = LogTypes.WARN;
                    baselog.Response = new { response = response!.Error };

                    return BadRequest(response!.Error);
                }

                var responseModel = new ResponseModel
                {
                    Message = "All done here, see your email to confirm your unsubscription.",
                    Code = "UR695"
                };

                baselog.Level = LogTypes.INFO;
                baselog.Response = new
                {
                    response = Encryptor.ObfuscateSensitiveData(JsonConvert.SerializeObject(response.Data)),
                    responseModel
                };

                return Ok(responseModel);
            }
            catch (ValidationException vex)
            {
                baselog.Level = LogTypes.WARN;
                return BadRequest(vex.Error);
            }
            catch (Exception ex)
            {
                baselog.Level = LogTypes.ERROR;
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "EX059" });
            }
            finally
            {
                await baselog.AddStepAsync("SUBSCRIPTION_REQUEST", log);
                await _logger.WriteLogAsync(baselog);
            }
        }
    }
}

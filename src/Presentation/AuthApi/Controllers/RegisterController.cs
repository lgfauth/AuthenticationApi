using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Validation;

namespace AuthApi.Controllers
{
    /// <summary>
    /// Registration controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        /// <summary>
        /// Registration controller constructor.
        /// </summary>
        /// <param name="registerService"></param>
        public RegisterController(IRegisterService registerService)
        {
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
            try
            {
                Validations.Validate(request);

                var response = await _registerService.RegisterAsync(request);

                return StatusCode(201, new ResponseModel
                {
                    Message = "All done here, see your email to confirm your subscription.",
                    Code = "SC201"
                });
            }
            catch (ValidationException vex)
            {
                return BadRequest(vex.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "EX058" });
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
            try
            {
                Validations.Validate(request);

                var response = await _registerService.UnregisterAsync(request);

                if (!response.IsSuccess)
                    return BadRequest(response!.Error);

                return Ok(new ResponseModel
                {
                    Message = "All done here, see your email to confirm your unsubscription.",
                    Code = "UR695"
                });
            }
            catch (ValidationException vex)
            {
                return BadRequest(vex.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message, Code = "EX059" });
            }
        }
    }
}

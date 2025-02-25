using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesApplication.Interfaces;
using ServicesApplication.Validators;

namespace AuthenticationApi.Controllers
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

                if (response is null)
                {
                    return StatusCode(500, new ResponseModel
                    {
                        Message = "An internal error has occurred. Please try again later.",
                        Code = "IE501"
                    });
                }
                else if (response.IsSuccess)
                {
                    return StatusCode(201, new ResponseModel
                    {
                        Message = "All done here, see your email to confirm your subscription.",
                        Code = "SC201"
                    });
                }

                return BadRequest(response!.Error);
            }
            catch(ValidationException vex)
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
        [HttpPost("unsubscribe")]
        [ProducesResponseType(typeof(ResponseModel), 201)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> Unregister([FromBody] UnsubscribeRequest request)
        {
            try
            {
                Validations.Validate(request);

                var response = await _registerService.UnregisterAsync(request);

                if (response is null)
                    return StatusCode(500, "An internal error has occurred. Please try again later.");
                else if (response.IsSuccess)
                    return Ok(new { Message = "All done here, see your email to confirm your unsubscription." });

                return BadRequest(response!.Error);
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
    }
}

using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesApplication.Interfaces;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost("subscription")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _registerService.RegisterAsync(request);

                if (response is null)
                    return StatusCode(500, "An internal error has occurred. Please try again later.");
                else if (response.IsSuccess)
                    return StatusCode(201, new { Message = "All done here, see your email to confirm your subscription." });

                return BadRequest(response!.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message, Code = "EX058" });
            }
        }

        [HttpPost("unsubscription")]
        public async Task<IActionResult> Unregister([FromBody] UnregisterRequest request)
        {
            try
            {
                var response = await _registerService.UnregisterAsync(request);
                
                if (response is null)
                    return StatusCode(500, "An internal error has occurred. Please try again later.");
                else if(response.IsSuccess)
                    return Ok(new { Message = "All done here, see your email to confirm your unsubscription." });
                
                return BadRequest(response!.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message, Code = "EX058" });
            }
        }
    }
}

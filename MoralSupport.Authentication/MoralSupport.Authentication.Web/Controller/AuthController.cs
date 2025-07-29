using Microsoft.AspNetCore.Mvc;
using MoralSupport.Authentication.Application.DTOs.Auth;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            try
            {
                var user = await _authService.AuthenticateWithGoogleAsync(request.IdToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Auth error: " + ex.Message); // DEBUG LINE
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}

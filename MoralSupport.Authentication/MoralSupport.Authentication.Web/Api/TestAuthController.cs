using Microsoft.AspNetCore.Mvc;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Api
{
    [ApiController]
    [Route("api/test-auth")]
    public class TestAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public TestAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string email)
        {
            var user = await _authService.AuthenticateWithFakeGoogleAsync(email);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserFromToken(string token)
        {
            var user = await _authService.GetUserFromTokenAsync(token);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
    }
}

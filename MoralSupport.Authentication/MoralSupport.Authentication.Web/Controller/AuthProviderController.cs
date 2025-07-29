using Microsoft.AspNetCore.Mvc;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Controller
{
    [Route("api/auth/provider")]
    [ApiController]
    public class AuthProviderController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthProviderController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet("google-client-id")]
        public async Task<IActionResult> GetGoogleClientId()
        {
            var clientId = await _authService.GetGoogleClientIdAsync();
            return Ok(new { clientId });
        }
    }
}

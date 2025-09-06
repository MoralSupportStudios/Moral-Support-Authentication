using Microsoft.AspNetCore.Mvc;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Controllers
{
    [ApiController]
    [Route("api/sso")]
    public class SsoController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ISessionStore _sessions;

        public SsoController(IAuthService auth, ISessionStore sessions)
        { _auth = auth; _sessions = sessions; }

        // Browser login => sets cookie + redirects back
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromForm] string idToken, [FromQuery] string? returnUrl)
        {
            var user = await _auth.AuthenticateWithGoogleAsync(idToken);

            var sid = await _sessions.CreateAsync(user.Id, TimeSpan.FromHours(8));
            Response.Cookies.Append("ms_sso", sid, new CookieOptions
            {
                Domain = ".moralsupportstudios.com",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });


            return Redirect(string.IsNullOrWhiteSpace(returnUrl)
                ? "https://core.mss.test:7242/Dashboard"
                : returnUrl);
        }

        public record IntrospectRequest(string SessionId);
        public record IntrospectResponse(int Id, string Email, string Name);

        // Server-to-server: Finance/Core ask “who is this session?”
        [HttpPost("introspect")]
        public async Task<IActionResult> Introspect([FromBody] IntrospectRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.SessionId))
            {
                return BadRequest();
            }

            var (ok, userId) = await _sessions.TryGetAsync(req.SessionId);
            if (!ok)
            {
                return Unauthorized();
            }

            var u = await _auth.GetUserByIdAsync(userId);
            return Ok(new IntrospectResponse(u.Id, u.Email, u.Name));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("ms_sso", out var sid))
            {
                await _sessions.DestroyAsync(sid);
            }

            Response.Cookies.Delete("ms_sso", new CookieOptions
            {
                Domain = ".mss.test",
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok();
        }
    }
}
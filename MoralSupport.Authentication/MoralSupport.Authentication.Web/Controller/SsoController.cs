using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Controllers;

[ApiController]
[Route("api/sso")]
public class SsoController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ISessionStore _sessions;
    private readonly SuiteOptions _opt;

    public SsoController(IAuthService auth, ISessionStore sessions, IOptions<SuiteOptions> opt)
    {
        _auth = auth;
        _sessions = sessions;
        _opt = opt.Value;
    }

    public record IntrospectRequest(string SessionId);
    public record IntrospectResponse(int Id, string Email, string Name);

    // Browser login => sets cookie + redirects back
    [HttpPost("google")]
    public async Task<IActionResult> Google([FromForm] string idToken, [FromQuery] string? returnUrl)
    {
        var user = await _auth.AuthenticateWithGoogleAsync(idToken);

        var sid = await _sessions.CreateAsync(user.Id, TimeSpan.FromHours(8));
        var isHttps = Request.IsHttps;
        Response.Cookies.Append(_opt.CookieName, sid, new CookieOptions
        {
            Domain = _opt.CookieDomain,
            Path = _opt.CookiePath,          // MUST match on delete
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });

        return Redirect(ResolveReturnUrl(returnUrl, _opt.LoginDefaultReturnUrl));
    }

    // Server-to-server: Finance/Core ask “who is this session?”
    [HttpPost("introspect")]
    public async Task<IActionResult> Introspect([FromBody] IntrospectRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.SessionId)) return BadRequest();
        var (ok, userId) = await _sessions.TryGetAsync(req.SessionId);
        if (!ok) return Unauthorized();

        var u = await _auth.GetUserByIdAsync(userId);
        return Ok(new IntrospectResponse(u.Id, u.Email, u.Name));
    }

    // Simple browser-friendly logout via GET (also provide POST)
    [HttpGet("logout")]
    public async Task<IActionResult> LogoutGet([FromQuery] string? returnUrl)
    {
        await InvalidateIfPresentAsync();
        DeleteCookie();
        return Redirect(ResolveReturnUrl(returnUrl, _opt.LogoutDefaultReturnUrl));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutPost()
    {
        await InvalidateIfPresentAsync();
        DeleteCookie();
        return Ok();
    }

    private async Task InvalidateIfPresentAsync()
    {
        if (Request.Cookies.TryGetValue(_opt.CookieName, out var sid) && !string.IsNullOrWhiteSpace(sid))
            await _sessions.DestroyAsync(sid);
    }

    private void DeleteCookie()
    {
        var isHttps = Request.IsHttps;
        Response.Cookies.Delete(_opt.CookieName, new CookieOptions
        {
            Domain = _opt.CookieDomain,
            Path = _opt.CookiePath,      // MUST match the set cookie
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax
        });
    }

    private string ResolveReturnUrl(string? returnUrl, string fallback)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return fallback;
        }

        if (Url.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
        {
            return fallback;
        }

        var allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (_opt.AllowedReturnHosts is { Length: > 0 })
        {
            foreach (var host in _opt.AllowedReturnHosts)
            {
                if (!string.IsNullOrWhiteSpace(host))
                {
                    allowedHosts.Add(host.Trim());
                }
            }
        }

        AddHostFromUrl(allowedHosts, _opt.LoginDefaultReturnUrl);
        AddHostFromUrl(allowedHosts, _opt.LogoutDefaultReturnUrl);

        return allowedHosts.Contains(uri.Host) ? returnUrl : fallback;
    }

    private static void AddHostFromUrl(ISet<string> hosts, string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            hosts.Add(uri.Host);
        }
    }
}

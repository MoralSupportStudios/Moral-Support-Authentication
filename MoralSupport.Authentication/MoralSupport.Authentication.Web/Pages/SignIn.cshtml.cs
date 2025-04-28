using Microsoft.AspNetCore.Mvc.RazorPages;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Pages
{
    public class SignInModel : PageModel
    {
        private readonly IAuthService _authService;

        public SignInModel(IAuthService authService)
        {
            _authService = authService;
        }

        public string GoogleLoginUrl { get; private set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var clientId = await _authService.GetGoogleClientIdAsync();

            GoogleLoginUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                             $"?client_id={clientId}" +
                             $"&redirect_uri=https://localhost:7241/signin-google" +
                             $"&response_type=id_token" +
                             $"&scope=openid%20email%20profile" +
                             $"&nonce=12345" +
                             $"&prompt=select_account";
        }
    }
}

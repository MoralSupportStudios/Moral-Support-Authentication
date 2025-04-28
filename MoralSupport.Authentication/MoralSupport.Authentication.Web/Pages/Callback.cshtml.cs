using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoralSupport.Authentication.Application.Interfaces;

namespace MoralSupport.Authentication.Web.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly IAuthService _authService;

        public CallbackModel(IAuthService authService)
        {
            _authService = authService;
        }

        public string? Message { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id_token)
        {
            if (string.IsNullOrEmpty(id_token))
            {
                Message = "No id_token received.";
                return Page();
            }

            var user = await _authService.AuthenticateWithGoogleAsync(id_token);
            Message = $"Welcome, {user.Name} ({user.Email})!";
            return Page();
        }
    }
}

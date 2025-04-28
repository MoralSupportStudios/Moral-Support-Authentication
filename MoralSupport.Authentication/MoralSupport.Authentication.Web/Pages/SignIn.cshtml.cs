using Microsoft.AspNetCore.Mvc;
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

        [BindProperty]
        public string Email { get; set; }

        public string? ResultMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(Email))
            {
                ResultMessage = "Invalid email.";
                return Page();
            }

            var user = await _authService.AuthenticateWithFakeGoogleAsync(Email);

            ResultMessage = $"Welcome, {user.Name} ({user.Email})!";
            return Page();
        }
    }
}

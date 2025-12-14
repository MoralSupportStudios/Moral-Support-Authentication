using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoralSupport.Authentication.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public string Version { get; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Prefer a deploy-time version from environment; fall back to a simple "dev" label.
            Version = _configuration["APP_VERSION"] ?? "dev";
        }

        public void OnGet()
        {

        }
    }
}

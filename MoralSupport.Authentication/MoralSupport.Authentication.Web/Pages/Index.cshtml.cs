using System.Reflection;
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
            var rawVersion = _configuration["APP_VERSION"];
            if (string.IsNullOrWhiteSpace(rawVersion))
            {
                rawVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                    ?? Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
            }

            Version = FormatVersion(rawVersion);
        }

        public void OnGet()
        {

        }

        private static string FormatVersion(string? rawVersion)
        {
            if (string.IsNullOrWhiteSpace(rawVersion))
            {
                return "V0.0.0.1";
            }

            var trimmed = rawVersion.Trim();
            if (trimmed.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed[1..];
            }

            var versionCore = trimmed.Split(new[] { '+', '-' }, 2)[0];
            if (System.Version.TryParse(versionCore, out var parsed))
            {
                var parts = new[] { parsed.Major, parsed.Minor, parsed.Build, parsed.Revision };
                var length = parsed.Revision >= 0 ? 4 : parsed.Build >= 0 ? 3 : 2;
                var formatted = string.Join('.', parts.Take(length));
                return $"V{formatted}";
            }

            return $"V{trimmed}";
        }
    }
}

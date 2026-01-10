namespace MoralSupport.Authentication.Web
{
    public sealed class SuiteOptions
    {
        public string CookieName { get; set; } = "ms_sso";
        public string CookieDomain { get; set; } = ".moralsupportstudios.com";
        public string CookiePath { get; set; } = "/";
        public string LoginDefaultReturnUrl { get; set; } = "https://core.moralsupportstudios.com:7242/Dashboard";
        public string LogoutDefaultReturnUrl { get; set; } = "https://core.moralsupportstudios.com:7242/SignIn";
        public string[] AllowedReturnHosts { get; set; } = Array.Empty<string>();
        public string[] AllowedReturnSchemes { get; set; } = new[] { "https" };
    }
}

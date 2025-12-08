namespace MoralSupport.Authentication.Domain.Entities
{
    public class ExternalProviderSettings
    {
        public int Id { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}

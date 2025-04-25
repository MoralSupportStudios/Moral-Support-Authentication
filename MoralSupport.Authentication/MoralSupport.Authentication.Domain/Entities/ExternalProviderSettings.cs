namespace MoralSupport.Authentication.Domain.Entities
{
    public class ExternalProviderSettings
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

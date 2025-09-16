namespace MoralSupport.Authentication.Domain.Entities
{
    public class SsoSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public int UserId { get; set; }
        public DateTime ExpiresUtc { get; set; }
    }
}
namespace MoralSupport.Authentication.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
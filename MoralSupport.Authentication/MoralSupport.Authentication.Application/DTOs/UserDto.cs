namespace MoralSupport.Authentication.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Provider { get; set; } = "Stub"; // Hardcoded for now
    }
}
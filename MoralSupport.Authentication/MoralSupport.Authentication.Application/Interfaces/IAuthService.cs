using MoralSupport.Authentication.Application.DTOs;

namespace MoralSupport.Authentication.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> AuthenticateWithFakeGoogleAsync(string fakeEmail);
        Task<UserDto?> GetUserFromTokenAsync(string token);
    }
}

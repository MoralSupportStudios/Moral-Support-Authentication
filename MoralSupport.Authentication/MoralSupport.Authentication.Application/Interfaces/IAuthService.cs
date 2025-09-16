using MoralSupport.Authentication.Application.DTOs;

namespace MoralSupport.Authentication.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> AuthenticateWithFakeGoogleAsync(string fakeEmail);
        Task<UserDto> AuthenticateWithGoogleAsync(string idToken);
        Task<UserDto?> GetUserFromTokenAsync(string token);
        Task<string> GetGoogleClientIdAsync();
        Task <UserDto>GetUserByIdAsync(int userId);
    }
}
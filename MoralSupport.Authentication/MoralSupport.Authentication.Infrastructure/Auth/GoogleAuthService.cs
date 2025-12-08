using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.DTOs;
using MoralSupport.Authentication.Application.Interfaces;
using MoralSupport.Authentication.Domain.Entities;
using MoralSupport.Authentication.Infrastructure.Persistence;

namespace MoralSupport.Authentication.Infrastructure.Auth
{
    public class GoogleAuthService : IAuthService
    {
        private readonly AuthenticationDbContext _dbContext;
        public GoogleAuthService(AuthenticationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserDto> AuthenticateWithFakeGoogleAsync(string fakeEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> AuthenticateWithGoogleAsync(string idToken)
        {
            var providerSettings = await LoadGoogleSettingsAsync();

            // Validating token
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { providerSettings.ClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null)
            {
                throw new Exception("Invalid Google token.");
            }

            //Find or create User
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Name = payload.Name ?? "Unknown",
                    Provider = "Google",
                    ProviderId = payload.Subject
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            //Returning User DTO
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Provider = user.Provider
            };
        }

        public Task<UserDto?> GetUserFromTokenAsync(string token)
        {
            return _dbContext.Users
                .Where(u => u.Email == token)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Provider = u.Provider
                })
                .FirstOrDefaultAsync();
        }
        public async Task<string> GetGoogleClientIdAsync()
        {
            var settings = await LoadGoogleSettingsAsync();
            return settings.ClientId;
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var dto = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Provider = u.Provider
                })
                .SingleOrDefaultAsync();

            if (dto is null)
            {
                throw new KeyNotFoundException($"User with id {userId} was not found.");
            }

            return dto;
        }

        private async Task<ExternalProviderSettings> LoadGoogleSettingsAsync()
        {
            var clientIdFromEnv = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var clientSecretFromEnv = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

            var providerSettings = await _dbContext.ExternalProviderSettings
                .FirstOrDefaultAsync(ps => ps.Provider == "Google");

            if (providerSettings == null)
            {
                throw new Exception("Google provider settings not found.");
            }

            if (!string.IsNullOrWhiteSpace(clientIdFromEnv))
            {
                providerSettings.ClientId = clientIdFromEnv;
            }

            if (!string.IsNullOrWhiteSpace(clientSecretFromEnv))
            {
                providerSettings.ClientSecret = clientSecretFromEnv;
            }

            return providerSettings;
        }
    }
}

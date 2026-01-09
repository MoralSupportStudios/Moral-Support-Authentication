using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.DTOs;
using MoralSupport.Authentication.Application.Interfaces;
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

        public async Task<UserDto> AuthenticateWithGoogleAsync(string idToken)
        {
            var clientId = GetRequiredEnv("GOOGLE_CLIENT_ID");

            // Validating token
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { clientId }
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
        public Task<string> GetGoogleClientIdAsync()
        {
            return Task.FromResult(GetRequiredEnv("GOOGLE_CLIENT_ID"));
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

        private static string GetRequiredEnv(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Missing required environment variable: {name}.");
            }

            return value;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.DTOs;
using MoralSupport.Authentication.Application.Interfaces;
using MoralSupport.Authentication.Domain.Entities;
using MoralSupport.Authentication.Infrastructure.Persistence;

namespace MoralSupport.Authentication.Infrastructure.Auth
{
    public class StubAuthService : IAuthService
    {
        private readonly AuthenticationDbContext _db;

        public StubAuthService(AuthenticationDbContext db)
        {
            _db = db;
        }

        public  async Task<UserDto> AuthenticateWithFakeGoogleAsync(string fakeEmail)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == fakeEmail);

            if (existingUser == null)
            {
                existingUser = new User
                {
                    Email = fakeEmail,
                    Name = "Test User",
                    Provider = "Stub",
                    ProviderId = Guid.NewGuid().ToString()
                };

                _db.Users.Add(existingUser);
                await _db.SaveChangesAsync();
            }

            return new UserDto
            {
                Id = existingUser.Id,
                Email = existingUser.Email,
                Name = existingUser.Name,
                Provider = existingUser.Provider
            };
        }

        public async Task<UserDto?> GetUserFromTokenAsync(string token)
        {
            // Fake token = email for now
            return await _db.Users
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
    }
}

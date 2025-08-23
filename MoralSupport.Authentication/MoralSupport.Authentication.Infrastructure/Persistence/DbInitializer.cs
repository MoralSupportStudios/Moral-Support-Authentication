using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Domain.Entities;

namespace MoralSupport.Authentication.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AuthenticationDbContext context)
        {
            if (!await context.ExternalProviderSettings.AnyAsync(p => p.Provider == "Google"))
            {
                context.ExternalProviderSettings.Add(new ExternalProviderSettings
                {
                    Provider = "Google",
                    ClientId = "insert-client-id-here-manually",
                    ClientSecret = "insert-secret-here-manually"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Domain.Entities;

namespace MoralSupport.Authentication.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AuthenticationDbContext context)
        {
            var googleSettings = await context.ExternalProviderSettings
                .FirstOrDefaultAsync(p => p.Provider == "Google");

            // Seed a placeholder row if it doesn't exist. Secrets should come from env, not be persisted.
            if (googleSettings == null)
            {
                context.ExternalProviderSettings.Add(new ExternalProviderSettings
                {
                    Provider = "Google",
                    ClientId = "set-with-env",
                    ClientSecret = "set-with-env"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}

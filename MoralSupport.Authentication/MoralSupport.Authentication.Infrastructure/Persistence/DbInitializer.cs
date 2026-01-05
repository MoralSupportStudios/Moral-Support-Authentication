namespace MoralSupport.Authentication.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static Task InitializeAsync(AuthenticationDbContext context)
        {
            _ = context;
            return Task.CompletedTask;
        }
    }
}

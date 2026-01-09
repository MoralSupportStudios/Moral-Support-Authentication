using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Infrastructure.Auth;
using MoralSupport.Authentication.Infrastructure.Persistence;

namespace MoralSupport.Authentication.Tests.Infrastructure;

[TestFixture]
public class GoogleAuthServiceTests
{
    private static AuthenticationDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AuthenticationDbContext(options);
    }

    [Test]
    public void GetGoogleClientIdAsync_Throws_WhenMissing()
    {
        var prior = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        try
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", null);
            using var db = CreateDbContext(Guid.NewGuid().ToString("n"));
            var service = new GoogleAuthService(db);

            Assert.That(async () => await service.GetGoogleClientIdAsync(),
                Throws.TypeOf<InvalidOperationException>());
        }
        finally
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", prior);
        }
    }

    [Test]
    public async Task GetGoogleClientIdAsync_ReturnsValue_WhenSet()
    {
        var prior = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        try
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "test-client-id");
            using var db = CreateDbContext(Guid.NewGuid().ToString("n"));
            var service = new GoogleAuthService(db);

            var value = await service.GetGoogleClientIdAsync();

            Assert.That(value, Is.EqualTo("test-client-id"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", prior);
        }
    }
}

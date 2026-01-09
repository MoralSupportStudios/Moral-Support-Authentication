using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Infrastructure.Auth;
using MoralSupport.Authentication.Infrastructure.Persistence;
using MoralSupport.Authentication.Domain.Entities;

namespace MoralSupport.Authentication.Tests.Infrastructure;

[TestFixture]
public class EfSessionStoreTests
{
    private static AuthenticationDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AuthenticationDbContext(options);
    }

    [Test]
    public async Task CreateAsync_PersistsSession_And_TryGetAsync_ReturnsUser()
    {
        var dbName = Guid.NewGuid().ToString("n");
        await using var db = CreateDbContext(dbName);
        var store = new EfSessionStore(db);

        var sessionId = await store.CreateAsync(42, TimeSpan.FromMinutes(5));
        var (ok, userId) = await store.TryGetAsync(sessionId);

        Assert.That(ok, Is.True);
        Assert.That(userId, Is.EqualTo(42));
    }

    [Test]
    public async Task TryGetAsync_ReturnsFalse_WhenExpired()
    {
        var dbName = Guid.NewGuid().ToString("n");
        await using var db = CreateDbContext(dbName);

        var expired = new SsoSession
        {
            UserId = 7,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(-1)
        };
        db.SsoSessions.Add(expired);
        await db.SaveChangesAsync();

        var store = new EfSessionStore(db);
        var (ok, userId) = await store.TryGetAsync(expired.Id);

        Assert.That(ok, Is.False);
        Assert.That(userId, Is.EqualTo(0));
        Assert.That(await db.SsoSessions.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task DestroyAsync_RemovesSession()
    {
        var dbName = Guid.NewGuid().ToString("n");
        await using var db = CreateDbContext(dbName);

        var session = new SsoSession
        {
            UserId = 11,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
        };
        db.SsoSessions.Add(session);
        await db.SaveChangesAsync();

        var store = new EfSessionStore(db);
        await store.DestroyAsync(session.Id);
        var (ok, userId) = await store.TryGetAsync(session.Id);

        Assert.That(ok, Is.False);
        Assert.That(userId, Is.EqualTo(0));
    }
}

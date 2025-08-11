using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using NUnit.Framework;
using MoralSupport.Authentication.Infrastructure.Persistence;
using MoralSupport.Authentication.Infrastructure.Auth;
using MoralSupport.Authentication.Domain.Entities;

namespace MoralSupport.Authentication.Tests
{
    public class StubAuthServiceTests
    {
        private static AuthenticationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            return new AuthenticationDbContext(options);
        }

        [Test]
        public async Task AuthenticateWithFakeGoogleAsync_CreatesUser_WhenNotExists()
        {
            var email = "newuser@example.com";
            await using var db = CreateDb();
            var sut = new StubAuthService(db);

            var dto = await sut.AuthenticateWithFakeGoogleAsync(email);

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.Email, Is.EqualTo(email));
            Assert.That(dto.Provider, Is.EqualTo("Stub"));
            Assert.That(await db.Users.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task AuthenticateWithFakeGoogleAsync_ReturnsExistingUser_WhenAlreadyExists()
        {
            var email = "exists@example.com";
            await using var db = CreateDb();
            var existing = new User
            {
                Email = email,
                Name = "Already Here",
                Provider = "Stub",
                ProviderId = Guid.NewGuid().ToString()
            };
            db.Users.Add(existing);
            await db.SaveChangesAsync();

            var sut = new StubAuthService(db);
            var dto = await sut.AuthenticateWithFakeGoogleAsync(email);

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.Id, Is.EqualTo(existing.Id));          
            Assert.That(await db.Users.CountAsync(), Is.EqualTo(1)); 
        }

        [Test]
        public async Task GetUserFromTokenAsync_ReturnsUser_WhenEmailMatches()
        {
            var email = "match@example.com";
            await using var db = CreateDb();
            var u = new User
            {
                Email = email,
                Name = "Match Me",
                Provider = "Stub",
                ProviderId = Guid.NewGuid().ToString()
            };
            db.Users.Add(u);
            await db.SaveChangesAsync();

            var sut = new StubAuthService(db);
            var dto = await sut.GetUserFromTokenAsync(email);

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto!.Id, Is.EqualTo(u.Id));
            Assert.That(dto.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task GetUserFromTokenAsync_ReturnsNull_WhenNoUserFound()
        {
            await using var db = CreateDb();
            var sut = new StubAuthService(db);

            var dto = await sut.GetUserFromTokenAsync("nobody@example.com");

            Assert.That(dto, Is.Null);
        }
    }
}

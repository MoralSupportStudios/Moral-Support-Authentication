using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.Interfaces;
using MoralSupport.Authentication.Domain.Entities;
using MoralSupport.Authentication.Infrastructure.Persistence;

namespace MoralSupport.Authentication.Infrastructure.Auth
{
    public sealed class EfSessionStore : ISessionStore
    {
        private readonly AuthenticationDbContext _db;
        public EfSessionStore(AuthenticationDbContext db) => _db = db;

        public async Task<string> CreateAsync(int userId, TimeSpan ttl, CancellationToken ct = default)
        {
            var s = new SsoSession
            {
                UserId = userId,
                ExpiresUtc = DateTime.UtcNow.Add(ttl)
            };
            _db.SsoSessions.Add(s);
            await _db.SaveChangesAsync(ct);
            return s.Id;
        }

        public async Task<(bool ok, int userId)> TryGetAsync(string sessionId, CancellationToken ct = default)
        {
            var s = await _db.SsoSessions.FirstOrDefaultAsync(x => x.Id == sessionId, ct);
            if (s is null || s.ExpiresUtc <= DateTime.UtcNow)
            {
                return (false, 0);
            }

            return (true, s.UserId);
        }

        public async Task DestroyAsync(string sessionId, CancellationToken ct = default)
        {
            var s = await _db.SsoSessions.FirstOrDefaultAsync(x => x.Id == sessionId, ct);
            if (s is null)
            {
                return;
            }

            _db.Remove(s);
            await _db.SaveChangesAsync(ct);
        }
    }
}
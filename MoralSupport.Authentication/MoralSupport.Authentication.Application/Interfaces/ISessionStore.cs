namespace MoralSupport.Authentication.Application.Interfaces
{
    public interface ISessionStore
    {
        Task<string> CreateAsync(int userId, TimeSpan ttl, CancellationToken ct = default);
        Task<(bool ok, int userId)> TryGetAsync(string sessionId, CancellationToken ct = default);
        Task DestroyAsync(string sessionId, CancellationToken ct = default);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.DTOs;            // UserDto
using MoralSupport.Authentication.Domain.Entities;            // User
using MoralSupport.Authentication.Infrastructure.Persistence;             // AuthenticationDbContext

namespace MoralSupport.Authentication.Web.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AuthenticationDbContext _db;
    public UsersController(AuthenticationDbContext db) => _db = db;

    // GET /api/users/2002
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id, CancellationToken ct)
    {
        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return u is null ? NotFound() : Ok(ToDto(u));
    }

    // POST /api/users/batch   body: [2002, 3003, ...]
    [HttpPost("batch")]
    public async Task<ActionResult<List<UserDto>>> Batch([FromBody] int[] ids, CancellationToken ct)
    {
        if (ids is null || ids.Length == 0)
        {
            return Ok(new List<UserDto>());
        }

        var unique = ids.Distinct().ToArray();

        var users = await _db.Users.AsNoTracking()
            .Where(u => unique.Contains(u.Id))
            .ToListAsync(ct);

        return Ok(users.Select(ToDto).ToList());
    }

    // GET /api/users/find-by-email?email=someone@example.com
    [HttpGet("find-by-email")]
    public async Task<ActionResult<UserDto>> FindByEmail([FromQuery] string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("email is required");
        }

        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);
        return u is null ? NotFound() : Ok(ToDto(u));
    }

    // GET /api/users/search?q=joey&take=10  (optional for future typeahead)
    [HttpGet("search")]
    public async Task<ActionResult<List<UserDto>>> Search([FromQuery] string q, [FromQuery] int take = 10, CancellationToken ct = default)
    {
        q ??= string.Empty;
        take = Math.Clamp(take, 1, 50);

        var users = await _db.Users.AsNoTracking()
            .Where(u => u.Email.Contains(q) || u.Name.Contains(q))
            .OrderBy(u => u.Id)
            .Take(take)
            .ToListAsync(ct);

        return Ok(users.Select(ToDto).ToList());
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        Name = u.Name,
        Provider = u.Provider
    };
}

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Application.Interfaces;
using MoralSupport.Authentication.Infrastructure.Auth;
using MoralSupport.Authentication.Infrastructure.Persistence;
using MoralSupport.Authentication.Web;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

ValidateRequiredEnvVars();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDbContext<AuthenticationDbContext>(options =>
{
    var connectionString = BuildConnectionString(builder.Configuration);
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory"));
});
builder.Services.AddScoped<IAuthService, GoogleAuthService>();
builder.Services.AddScoped<ISessionStore, EfSessionStore>();
builder.Services.Configure<SuiteOptions>(builder.Configuration.GetSection("Suite"));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


var app = builder.Build();

// Ensure database is up to date on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();

static void ValidateRequiredEnvVars()
{
    var required = new[]
    {
        "GOOGLE_CLIENT_ID"
    };

    var missing = required.Where(name => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)))
        .ToArray();

    if (missing.Length > 0)
    {
        throw new InvalidOperationException(
            $"Missing required environment variable(s): {string.Join(", ", missing)}.");
    }
}

static string BuildConnectionString(ConfigurationManager config)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Username = userInfo.FirstOrDefault() ?? string.Empty,
            Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };

        // Preserve any query parameters in DATABASE_URL (e.g., sslmode=require)
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        foreach (var pair in query)
        {
            // Only overwrite if the key isn't already populated
            if (!builder.ContainsKey(pair.Key))
            {
                builder[pair.Key] = pair.Value.ToString();
            }
        }

        return builder.ConnectionString;
    }

    var cs = config.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs))
    {
        throw new InvalidOperationException("No database connection string configured. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
    }

    return cs;
}

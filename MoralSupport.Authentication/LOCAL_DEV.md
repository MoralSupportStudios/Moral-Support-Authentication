# Local Development (Auth + Core)

This repo is configured for PostgreSQL (Npgsql) in the auth service. For local
development, run a local Postgres instance and point the auth app at it via
`appsettings.Development.json` or User Secrets.

## 1) Start Postgres locally

Example with Docker:

```
docker run --name moral-support-auth-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=moral_support_auth \
  -p 5432:5432 \
  -d postgres:16
```

## 2) Configure Auth app (local)

Create or update `MoralSupport.Authentication.Web/appsettings.Development.json`:

```json
{
  "DetailedErrors": true,
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=moral_support_auth;Username=postgres;Password=postgres"
  },
  "Suite": {
    "CookieDomain": "",
    "CookiePath": "/",
    "LoginDefaultReturnUrl": "https://localhost:7242/",
    "LogoutDefaultReturnUrl": "https://localhost:7242/"
  }
}
```

Set secrets for Google auth (User Secrets or environment variables). Do not
commit secrets to the repo:

- `GOOGLE_CLIENT_ID`
- `GOOGLE_CLIENT_SECRET`

User Secrets example:

```
dotnet user-secrets init --project MoralSupport.Authentication.Web
dotnet user-secrets set "GOOGLE_CLIENT_ID" "your-client-id" --project MoralSupport.Authentication.Web
dotnet user-secrets set "GOOGLE_CLIENT_SECRET" "your-client-secret" --project MoralSupport.Authentication.Web
```

Note: Add a local OAuth redirect URI in your Google OAuth app, such as
`https://localhost:7241/signin-google` (adjust port if needed).

## 3) Configure Core app (local)

In the Core app `appsettings.Development.json`, point auth to local:

```json
{
  "MoralSupportApps": {
    "Authentication": "https://localhost:7241/"
  }
}
```

## 4) Run locally

Start the auth app (it will apply migrations on startup):

```
dotnet run --project MoralSupport.Authentication.Web
```

Start your Core app and any other services, then log in against the local auth
service.

## Notes

- Ports `7241` and `7242` are placeholders; adjust to your actual launch settings.
- If you use HTTPS locally, ensure your dev certificates are trusted.
- Keep prod secrets in Fly; keep local secrets in User Secrets or local env vars.

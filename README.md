# Moral Support Authentication

Centralized auth for the Moral Support Studios suite. Google SSO + session tokens for all apps, backed by EF Core and Postgres.

## What's in here?
- `.NET 8` API with EF Core migrations tuned for Postgres (Supabase-ready).
- `Dockerfile` + `fly.toml` for Fly.io deployment.
- GitHub Actions: marketing site to Pages and API deploy to Fly on every push to `main`.
- Auto-migrations + Google provider seeding on startup (`DATABASE_URL`, `GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`).

## Environment (prod + CI)
Set these as secrets in Fly and GitHub (Actions/Pages):
```
DATABASE_URL=<postgres connection string from Supabase>
GOOGLE_CLIENT_ID=<google client id>
GOOGLE_CLIENT_SECRET=<google client secret>
Suite__CookieName=ms_sso
Suite__CookieDomain=.moralsupportstudios.com
Suite__CookiePath=/
Suite__LoginDefaultReturnUrl=https://core.moralsupportstudios.com/Dashboard
Suite__LogoutDefaultReturnUrl=https://core.moralsupportstudios.com/SignIn
```
For CI deploys you also need `FLY_API_TOKEN` in GitHub secrets (flyctl auth token).

## Local development
1) Install Postgres locally (or run Supabase locally) and create a database, e.g. `moralsupport_auth`.
2) Set `ConnectionStrings__DefaultConnection` (or `DATABASE_URL`) to that database.
3) Run the app; it will apply migrations + seed Google provider if missing:
```
dotnet run --project MoralSupport.Authentication/MoralSupport.Authentication.Web
```
Swagger lives at `/swagger`.

## Deploy API to Fly.io (backed by Supabase)
1) Update `fly.toml` `app` name and (optionally) `primary_region`.
2) Log in and set secrets:
```
flyctl auth login
flyctl secrets set DATABASE_URL="postgresql://..." GOOGLE_CLIENT_ID="..." GOOGLE_CLIENT_SECRET="..." \
  Suite__CookieName="ms_sso" Suite__CookieDomain=".moralsupportstudios.com" Suite__CookiePath="/" \
  Suite__LoginDefaultReturnUrl="https://core.moralsupportstudios.com/Dashboard" \
  Suite__LogoutDefaultReturnUrl="https://core.moralsupportstudios.com/SignIn"
```
3) Deploy manually (optional, CI will also deploy):
```
flyctl deploy --dockerfile Dockerfile
```
The app listens on `:8080`; Fly routes ports 80/443 via `fly.toml`.

### Fly.io + Supabase setup checklist
- Create Supabase project → copy `DATABASE_URL` (contains `sslmode=require`).
- Create Fly app: `flyctl apps create <your-app-name>`.
- Set Fly secrets (see above); do **not** commit secrets.
- Restrict Supabase DB access to the Fly IP range if desired (optional, Supabase defaults are restricted).
- First deploy will run EF migrations automatically.

## CI/CD (GitHub Actions)
- `.github/workflows/api-deploy.yml` builds and deploys the API to Fly on pushes to `main` using `FLY_API_TOKEN`.
- `.github/workflows/pages.yml` publishes `marketing/` to GitHub Pages on pushes to `main`.
- Enable GitHub Pages in repo settings and choose "GitHub Actions" as the source.

### GitHub secrets required
- `FLY_API_TOKEN`: Fly deploy token.
- (Optional) duplicate runtime env vars if you add build-time integration tests.

## Database (Supabase)
- Create a Supabase project and copy the `DATABASE_URL` (it includes `sslmode=require`).
- No manual SQL needed: the API runs `db.Database.Migrate()` on startup and seeds Google provider credentials from env.
- To run migrations manually:
```
dotnet ef database update --project MoralSupport.Authentication/MoralSupport.Authentication.Infrastructure --startup-project MoralSupport.Authentication/MoralSupport.Authentication.Web
```

## Production hardening checklist
- Ensure `DATABASE_URL`, `GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`, and suite cookie settings are set as Fly secrets.
- Verify domain/cookie settings in `appsettings.json` match production hosts.
- Remove or gate any non-prod endpoints (already removed: `/api/test-auth`).
- Keep Swagger off the public internet unless explicitly intended; add auth if you expose it.
- Secrets are **not** stored in source; Google secrets are read from env at runtime (DB holds placeholders only). Rotate credentials if exposure is suspected.

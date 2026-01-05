-- Enable RLS for tables flagged by Supabase Security Advisor.
alter table if exists public."__EFMigrationsHistory" enable row level security;
alter table if exists public."Users" enable row level security;
alter table if exists public."SsoSessions" enable row level security;

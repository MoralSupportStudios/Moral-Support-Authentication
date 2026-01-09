using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoralSupport.Authentication.Infrastructure.Persistence.Migrations
{
    public partial class MoveMigrationsHistorySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "migrations");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = 'public'
          AND c.relname = '__EFMigrationsHistory'
    ) THEN
        EXECUTE 'ALTER TABLE public.""__EFMigrationsHistory"" SET SCHEMA migrations';
    END IF;
END $$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = 'migrations'
          AND c.relname = '__EFMigrationsHistory'
    ) THEN
        EXECUTE 'ALTER TABLE migrations.""__EFMigrationsHistory"" SET SCHEMA public';
    END IF;
END $$;
");
        }
    }
}

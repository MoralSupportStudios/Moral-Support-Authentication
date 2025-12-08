using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoralSupport.Authentication.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSsoSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SsoSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpiresUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SsoSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SsoSessions_UserId",
                table: "SsoSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SsoSessions");
        }
    }
}

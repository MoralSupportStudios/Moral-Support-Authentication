using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MoralSupport.Authentication.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AuthenticationDbContext))]
    [Migration("20260109000000_RemoveExternalProviderSettings")]
    public partial class RemoveExternalProviderSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalProviderSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalProviderSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecret = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProviderSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviderSettings_Provider",
                table: "ExternalProviderSettings",
                column: "Provider",
                unique: true);
        }
    }
}

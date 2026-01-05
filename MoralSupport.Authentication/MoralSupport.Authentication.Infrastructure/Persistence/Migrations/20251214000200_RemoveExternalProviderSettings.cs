using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MoralSupport.Authentication.Infrastructure.Persistence;

#nullable disable

namespace MoralSupport.Authentication.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AuthenticationDbContext))]
    [Migration("20251214000200_RemoveExternalProviderSettings")]
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

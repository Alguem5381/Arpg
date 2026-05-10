using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arpg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePlayerIdsToUserIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerIds",
                table: "GameTables",
                newName: "UserIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserIds",
                table: "GameTables",
                newName: "PlayerIds");
        }
    }
}

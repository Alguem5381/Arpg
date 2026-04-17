using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arpg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sheets_Users_UserId",
                table: "Sheets");

            migrationBuilder.DropIndex(
                name: "IX_Sheets_UserId",
                table: "Sheets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Sheets");

            migrationBuilder.CreateTable(
                name: "GameTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DmId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    SheetIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTables", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTables");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Sheets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sheets_UserId",
                table: "Sheets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sheets_Users_UserId",
                table: "Sheets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

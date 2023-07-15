using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class AddRoleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                schema: "MangaHome",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "MangaHome",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "MangaHome",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                schema: "MangaHome",
                table: "Users",
                column: "RoleId",
                principalSchema: "MangaHome",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "MangaHome");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                schema: "MangaHome",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

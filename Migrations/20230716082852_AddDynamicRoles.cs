using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class AddDynamicRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "MangaHome",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UserId",
                schema: "MangaHome",
                table: "Roles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_UserId",
                schema: "MangaHome",
                table: "Roles",
                column: "UserId",
                principalSchema: "MangaHome",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_UserId",
                schema: "MangaHome",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_UserId",
                schema: "MangaHome",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "MangaHome",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                schema: "MangaHome",
                table: "Users",
                type: "text",
                nullable: true);

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
    }
}

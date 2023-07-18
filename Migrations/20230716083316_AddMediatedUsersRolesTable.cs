using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class AddMediatedUsersRolesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "RoleUser",
                schema: "MangaHome",
                columns: table => new
                {
                    RolesId = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "MangaHome",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "MangaHome",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                schema: "MangaHome",
                table: "RoleUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleUser",
                schema: "MangaHome");

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
    }
}

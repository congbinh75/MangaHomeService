using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class ChangeTitleOtherName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TitleOtherName_LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TitleOtherName_Language_LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName",
                column: "LanguageId",
                principalSchema: "MangaHome",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TitleOtherName_Language_LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName");

            migrationBuilder.DropIndex(
                name: "IX_TitleOtherName_LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                schema: "MangaHome",
                table: "TitleOtherName");
        }
    }
}

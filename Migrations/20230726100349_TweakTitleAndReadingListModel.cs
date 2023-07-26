using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class TweakTitleAndReadingListModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                schema: "MangaHome",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TitleOtherName",
                schema: "MangaHome",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TitleId = table.Column<string>(type: "text", nullable: false),
                    OtherName = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleOtherName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitleOtherName_Titles_TitleId",
                        column: x => x.TitleId,
                        principalSchema: "MangaHome",
                        principalTable: "Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThemeTitle",
                schema: "MangaHome",
                columns: table => new
                {
                    ThemesId = table.Column<string>(type: "text", nullable: false),
                    TitlesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThemeTitle", x => new { x.ThemesId, x.TitlesId });
                    table.ForeignKey(
                        name: "FK_ThemeTitle_Themes_ThemesId",
                        column: x => x.ThemesId,
                        principalSchema: "MangaHome",
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThemeTitle_Titles_TitlesId",
                        column: x => x.TitlesId,
                        principalSchema: "MangaHome",
                        principalTable: "Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThemeTitle_TitlesId",
                schema: "MangaHome",
                table: "ThemeTitle",
                column: "TitlesId");

            migrationBuilder.CreateIndex(
                name: "IX_TitleOtherName_TitleId",
                schema: "MangaHome",
                table: "TitleOtherName",
                column: "TitleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThemeTitle",
                schema: "MangaHome");

            migrationBuilder.DropTable(
                name: "TitleOtherName",
                schema: "MangaHome");

            migrationBuilder.DropTable(
                name: "Themes",
                schema: "MangaHome");
        }
    }
}

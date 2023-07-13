using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaHomeService.Migrations
{
    public partial class AddBaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MangaHome");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "MangaHome");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "MangaHome",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                schema: "MangaHome",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "MangaHome",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                schema: "MangaHome",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                schema: "MangaHome",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "MangaHome",
                newName: "Users");
        }
    }
}

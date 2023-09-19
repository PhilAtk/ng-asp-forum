using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class EditedTagAndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "dateModified",
                table: "Threads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "edited",
                table: "Threads",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "dateModified",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "edited",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateModified",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "edited",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "dateModified",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "edited",
                table: "Posts");
        }
    }
}

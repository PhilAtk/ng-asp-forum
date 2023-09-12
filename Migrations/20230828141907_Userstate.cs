using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp.Migrations
{
    /// <inheritdoc />
    public partial class Userstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userState",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userState",
                table: "Users");
        }
    }
}

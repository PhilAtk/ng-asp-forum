using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "userRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userRole",
                table: "Users",
                newName: "role");
        }
    }
}

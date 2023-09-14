using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Users_authoruserID",
                table: "Threads");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "authoruserID",
                table: "Threads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Users_authoruserID",
                table: "Threads",
                column: "authoruserID",
                principalTable: "Users",
                principalColumn: "userID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Users_authoruserID",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "code",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "authoruserID",
                table: "Threads",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Users_authoruserID",
                table: "Threads",
                column: "authoruserID",
                principalTable: "Users",
                principalColumn: "userID");
        }
    }
}

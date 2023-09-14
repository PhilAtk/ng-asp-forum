using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesReferenceByObjectInsteadOfID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userID",
                table: "Threads",
                newName: "authoruserID");

            migrationBuilder.RenameColumn(
                name: "userID",
                table: "Posts",
                newName: "authoruserID");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_authoruserID",
                table: "Threads",
                column: "authoruserID");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_authoruserID",
                table: "Posts",
                column: "authoruserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_authoruserID",
                table: "Posts",
                column: "authoruserID",
                principalTable: "Users",
                principalColumn: "userID",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Posts_Users_authoruserID",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Users_authoruserID",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_authoruserID",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Posts_authoruserID",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "authoruserID",
                table: "Threads",
                newName: "userID");

            migrationBuilder.RenameColumn(
                name: "authoruserID",
                table: "Posts",
                newName: "userID");
        }
    }
}

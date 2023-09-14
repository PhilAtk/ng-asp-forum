using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class RelateThreadAndPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_threadID",
                table: "Posts",
                column: "threadID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Threads_threadID",
                table: "Posts",
                column: "threadID",
                principalTable: "Threads",
                principalColumn: "threadID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Threads_threadID",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_threadID",
                table: "Posts");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class UserAuditsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAudits",
                columns: table => new
                {
                    auditID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    userID = table.Column<int>(type: "INTEGER", nullable: false),
                    action = table.Column<int>(type: "INTEGER", nullable: false),
                    info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAudits", x => x.auditID);
                    table.ForeignKey(
                        name: "FK_UserAudits_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAudits_userID",
                table: "UserAudits",
                column: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAudits");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ng_asp_forum.Migrations
{
    /// <inheritdoc />
    public partial class ThreadAudits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreadAudits",
                columns: table => new
                {
                    auditID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    threadID = table.Column<int>(type: "INTEGER", nullable: false),
                    action = table.Column<int>(type: "INTEGER", nullable: false),
                    info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadAudits", x => x.auditID);
                    table.ForeignKey(
                        name: "FK_ThreadAudits_Threads_threadID",
                        column: x => x.threadID,
                        principalTable: "Threads",
                        principalColumn: "threadID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreadAudits_threadID",
                table: "ThreadAudits",
                column: "threadID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreadAudits");
        }
    }
}

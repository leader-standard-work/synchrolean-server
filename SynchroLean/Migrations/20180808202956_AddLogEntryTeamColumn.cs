using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddLogEntryTeamColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TaskCompletionLog",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_TeamId",
                table: "TaskCompletionLog",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionLog_Teams_TeamId",
                table: "TaskCompletionLog",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionLog_Teams_TeamId",
                table: "TaskCompletionLog");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompletionLog_TeamId",
                table: "TaskCompletionLog");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TaskCompletionLog");
        }
    }
}

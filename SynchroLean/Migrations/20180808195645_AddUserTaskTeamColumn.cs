using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddUserTaskTeamColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "UserTasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TeamId",
                table: "UserTasks",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_TeamId",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "UserTasks");
        }
    }
}

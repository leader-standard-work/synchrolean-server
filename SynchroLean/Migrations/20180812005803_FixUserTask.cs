using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class FixUserTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerEmail",
                table: "UserTasks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks",
                column: "OwnerEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerEmail",
                table: "UserTasks",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks",
                column: "OwnerEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

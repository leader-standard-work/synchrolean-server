using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddUserAccountTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_UserAccounts_OwnerId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_UserTasks_TaskId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Todos",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_OwnerId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_TaskId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Todos");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TaskCompletionLog",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Todos",
                table: "Todos",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_OwnerId",
                table: "UserTasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_TeamId",
                table: "TaskCompletionLog",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests",
                column: "InviterOwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionLog_Teams_TeamId",
                table: "TaskCompletionLog",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_Tasks_TaskId",
                table: "Todos",
                column: "TaskId",
                principalTable: "UserTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerId",
                table: "UserTasks",
                column: "OwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionLog_Teams_TeamId",
                table: "TaskCompletionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Tasks_TaskId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerId",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_OwnerId",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Todos",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompletionLog_TeamId",
                table: "TaskCompletionLog");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TaskCompletionLog");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "UserTasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "UserTasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "UserAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Todos",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Todos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Todos",
                table: "Todos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_OwnerId",
                table: "Todos",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TaskId",
                table: "Todos",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests",
                column: "InviterOwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_UserAccounts_OwnerId",
                table: "Todos",
                column: "OwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_UserTasks_TaskId",
                table: "Todos",
                column: "TaskId",
                principalTable: "UserTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Teams_TeamId",
                table: "UserTasks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

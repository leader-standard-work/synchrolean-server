using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class ChangeUserAccountKeyToEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviteeOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionLog_UserAccounts_OwnerId",
                table: "TaskCompletionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_UserAccounts_MemberId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_OwnerId",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_MemberId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCompletionLog",
                table: "TaskCompletionLog");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompletionLog_OwnerId",
                table: "TaskCompletionLog");

            migrationBuilder.DropIndex(
                name: "IX_AddUserRequests_InviteeOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropIndex(
                name: "IX_AddUserRequests_InviterOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TaskCompletionLog");

            migrationBuilder.DropColumn(
                name: "InviteeOwnerId",
                table: "AddUserRequests");

            migrationBuilder.DropColumn(
                name: "InviterOwnerId",
                table: "AddUserRequests");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "UserTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Teams",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemberEmail",
                table: "TeamMembers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "TaskCompletionLog",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InviteeEmail",
                table: "AddUserRequests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InviterEmail",
                table: "AddUserRequests",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "MemberEmail" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCompletionLog",
                table: "TaskCompletionLog",
                columns: new[] { "TaskId", "OwnerEmail", "EntryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_OwnerEmail",
                table: "UserTasks",
                column: "OwnerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_MemberEmail",
                table: "TeamMembers",
                column: "MemberEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_OwnerEmail",
                table: "TaskCompletionLog",
                column: "OwnerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviteeEmail",
                table: "AddUserRequests",
                column: "InviteeEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviterEmail",
                table: "AddUserRequests",
                column: "InviterEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviteeEmail",
                table: "AddUserRequests",
                column: "InviteeEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterEmail",
                table: "AddUserRequests",
                column: "InviterEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionLog_UserAccounts_OwnerEmail",
                table: "TaskCompletionLog",
                column: "OwnerEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_UserAccounts_MemberEmail",
                table: "TeamMembers",
                column: "MemberEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks",
                column: "OwnerEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviteeEmail",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterEmail",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionLog_UserAccounts_OwnerEmail",
                table: "TaskCompletionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_UserAccounts_MemberEmail",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerEmail",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_OwnerEmail",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_MemberEmail",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCompletionLog",
                table: "TaskCompletionLog");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompletionLog_OwnerEmail",
                table: "TaskCompletionLog");

            migrationBuilder.DropIndex(
                name: "IX_AddUserRequests_InviteeEmail",
                table: "AddUserRequests");

            migrationBuilder.DropIndex(
                name: "IX_AddUserRequests_InviterEmail",
                table: "AddUserRequests");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "MemberEmail",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "TaskCompletionLog");

            migrationBuilder.DropColumn(
                name: "InviteeEmail",
                table: "AddUserRequests");

            migrationBuilder.DropColumn(
                name: "InviterEmail",
                table: "AddUserRequests");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "UserTasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "UserAccounts",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "TeamMembers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "TaskCompletionLog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InviteeOwnerId",
                table: "AddUserRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InviterOwnerId",
                table: "AddUserRequests",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts",
                column: "OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "MemberId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCompletionLog",
                table: "TaskCompletionLog",
                columns: new[] { "TaskId", "OwnerId", "EntryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_OwnerId",
                table: "UserTasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_MemberId",
                table: "TeamMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_OwnerId",
                table: "TaskCompletionLog",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviteeOwnerId",
                table: "AddUserRequests",
                column: "InviteeOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviterOwnerId",
                table: "AddUserRequests",
                column: "InviterOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviteeOwnerId",
                table: "AddUserRequests",
                column: "InviteeOwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                table: "AddUserRequests",
                column: "InviterOwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionLog_UserAccounts_OwnerId",
                table: "TaskCompletionLog",
                column: "OwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_UserAccounts_MemberId",
                table: "TeamMembers",
                column: "MemberId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_UserAccounts_OwnerId",
                table: "UserTasks",
                column: "OwnerId",
                principalTable: "UserAccounts",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class InviteRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_Teams_DestinationTeamId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviteeEmail",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequests_UserAccounts_InviterEmail",
                table: "AddUserRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddUserRequests",
                table: "AddUserRequests");

            migrationBuilder.DropIndex(
                name: "IX_AddUserRequests_InviteeEmail",
                table: "AddUserRequests");

            migrationBuilder.DropColumn(
                name: "AddUserRequestId",
                table: "AddUserRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddUserRequests",
                table: "AddUserRequests",
                columns: new[] { "InviteeEmail", "DestinationTeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequest_Team_TeamId",
                table: "AddUserRequests",
                column: "DestinationTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequest_InviteeEmail_UserAccount_Email",
                table: "AddUserRequests",
                column: "InviteeEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequest_InviterEmail_UserAccount_Email",
                table: "AddUserRequests",
                column: "InviterEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequest_Team_TeamId",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequest_InviteeEmail_UserAccount_Email",
                table: "AddUserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AddUserRequest_InviterEmail_UserAccount_Email",
                table: "AddUserRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddUserRequests",
                table: "AddUserRequests");

            migrationBuilder.AddColumn<int>(
                name: "AddUserRequestId",
                table: "AddUserRequests",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddUserRequests",
                table: "AddUserRequests",
                column: "AddUserRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviteeEmail",
                table: "AddUserRequests",
                column: "InviteeEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_AddUserRequests_Teams_DestinationTeamId",
                table: "AddUserRequests",
                column: "DestinationTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }
    }
}

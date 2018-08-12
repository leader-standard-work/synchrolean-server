using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class ReleaseCandidateOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerEmail = table.Column<string>(nullable: false),
                    TeamName = table.Column<string>(maxLength: 25, nullable: false),
                    TeamDescription = table.Column<string>(maxLength: 250, nullable: true),
                    Deleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "TeamPermissions",
                columns: table => new
                {
                    SubjectTeamId = table.Column<int>(nullable: false),
                    ObjectTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPermissions", x => new { x.SubjectTeamId, x.ObjectTeamId });
                    table.ForeignKey(
                        name: "FK_TeamPermissions_Teams_ObjectTeamId",
                        column: x => x.ObjectTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPermissions_Teams_SubjectTeamId",
                        column: x => x.SubjectTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddUserRequests",
                columns: table => new
                {
                    DestinationTeamId = table.Column<int>(nullable: false),
                    InviteeEmail = table.Column<string>(nullable: false),
                    InviterEmail = table.Column<string>(nullable: true),
                    IsAuthorized = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddUserRequests", x => new { x.InviteeEmail, x.DestinationTeamId });
                    table.ForeignKey(
                        name: "FK_AddUserRequest_Team_TeamId",
                        column: x => x.DestinationTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddUserRequest_InviteeEmail_UserAccount_Email",
                        column: x => x.InviteeEmail,
                        principalTable: "UserAccounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddUserRequest_InviterEmail_UserAccount_Email",
                        column: x => x.InviterEmail,
                        principalTable: "UserAccounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false),
                    MemberEmail = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => new { x.TeamId, x.MemberEmail });
                    table.ForeignKey(
                        name: "FK_TeamMembers_UserAccounts_MemberEmail",
                        column: x => x.MemberEmail,
                        principalTable: "UserAccounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsRecurring = table.Column<bool>(nullable: false),
                    Weekdays = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    OwnerEmail = table.Column<string>(nullable: true),
                    Frequency = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: true),
                    Deleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasks_UserAccounts_OwnerEmail",
                        column: x => x.OwnerEmail,
                        principalTable: "UserAccounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTasks_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskCompletionLog",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false),
                    OwnerEmail = table.Column<string>(nullable: false),
                    EntryTime = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    TeamId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCompletionLog", x => new { x.TaskId, x.OwnerEmail, x.EntryTime });
                    table.ForeignKey(
                        name: "FK_TaskCompletionLog_UserAccounts_OwnerEmail",
                        column: x => x.OwnerEmail,
                        principalTable: "UserAccounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCompletionLog_UserTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "UserTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCompletionLog_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false),
                    Completed = table.Column<DateTime>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Todo_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "UserTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_DestinationTeamId",
                table: "AddUserRequests",
                column: "DestinationTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviterEmail",
                table: "AddUserRequests",
                column: "InviterEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_OwnerEmail",
                table: "TaskCompletionLog",
                column: "OwnerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionLog_TeamId",
                table: "TaskCompletionLog",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_MemberEmail",
                table: "TeamMembers",
                column: "MemberEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_ObjectTeamId",
                table: "TeamPermissions",
                column: "ObjectTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_OwnerEmail",
                table: "UserTasks",
                column: "OwnerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TeamId",
                table: "UserTasks",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddUserRequests");

            migrationBuilder.DropTable(
                name: "TaskCompletionLog");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "TeamPermissions");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "UserTasks");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
